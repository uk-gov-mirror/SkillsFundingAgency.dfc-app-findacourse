﻿using AutoMapper;
using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Helpers;
using DFC.App.FindACourse.Services;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class BaseController
    {
        protected const string DefaultHelpArticleName = "help";
        protected const string testContentId = "87dfb08e-13ec-42ff-9405-5bbde048827a";


        public BaseController()
        {
            FakeLogService = A.Fake<ILogService>();
            FakeFindACoursesService = A.Fake<IFindACourseService>();
            FakeStaticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            CmsApiClientOptions = new CmsApiClientOptions() { ContentIds = testContentId };
            FakeMapper = A.Fake<IMapper>();
            FakeViewHelper = A.Fake<IViewHelper>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogService FakeLogService { get; }

        protected IFindACourseService FakeFindACoursesService { get; }

        protected IViewHelper FakeViewHelper { get; }

        protected IMapper FakeMapper { get; }

        protected IDocumentService<StaticContentItemModel> FakeStaticContentDocumentService { get; set; }

        protected CmsApiClientOptions CmsApiClientOptions { get; set; }

        protected CourseController BuildCourseController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();
            var fakeTempDataProvider = A.Fake<ITempDataProvider>();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;
            httpContext.RequestServices = A.Fake<IServiceProvider>();

            var controller = new CourseController(FakeLogService, FakeFindACoursesService, FakeViewHelper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
                TempData = new TempDataDictionary(httpContext, fakeTempDataProvider),
            };

            return controller;
        }

        protected DetailsController BuildDetailsController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            A.CallTo(() => FakeStaticContentDocumentService.GetByIdAsync(A<Guid>.Ignored, null)).Returns(new StaticContentItemModel() { Title = nameof(StaticContentItemModel.Title) });

            var controller = new DetailsController(FakeLogService, FakeFindACoursesService, FakeStaticContentDocumentService, CmsApiClientOptions, FakeMapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
