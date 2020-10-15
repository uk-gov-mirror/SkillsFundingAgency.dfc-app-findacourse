﻿using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class DetailsController : Controller
    {
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;

        public DetailsController(ILogService logService, IFindACourseService findACourseService)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
        }

        [HttpGet]
        [Route("find-a-course/search/details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string searchTerm, string CurrentSearchTerm, string town, string courseType,
                                                      string courseHours, string courseStudyTime, string courseStartDate, string distance, string filtera, int page)
        {
            this.logService.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            if (searchTerm == null && CurrentSearchTerm != null)
            {
                searchTerm = CurrentSearchTerm;
            }

            model.SearchTerm = $"searchTerm={searchTerm}&town={town}&courseType={courseType}&courseHours={courseHours}&studyTime={courseStudyTime}&startDate={courseStartDate}&distance={distance}&filtera={filtera}&page={page}";

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
               model.CourseDetails = await this.findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }
    }
}