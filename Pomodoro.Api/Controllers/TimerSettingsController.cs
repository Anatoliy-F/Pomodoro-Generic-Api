﻿// <copyright file="TimerSettingsController.cs" company="PomodoroGroup_GL_BaseCamp">
// Copyright (c) PomodoroGroup_GL_BaseCamp. All rights reserved.
// </copyright>

using Pomodoro.Api.Controllers.Base;
using Pomodoro.Dal.Entities;
using Pomodoro.Dal.Repositories.Interfaces;
using Pomodoro.Services.Interfaces;
using Pomodoro.Services.Models;

namespace Pomodoro.Api.Controllers
{
    /// <summary>
    /// Manage tracker settings.
    /// </summary>
    public class TimerSettingsController : BaseCrudController<ITimerSettingsService, TimerSettings, TimerSettingsModel, ITimerSettingRepository>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimerSettingsController"/> class.
        /// </summary>
        /// <param name="service">Instance of TimerSettings service.</param>
        public TimerSettingsController(ITimerSettingsService service)
            : base(service)
        {
        }
    }
}
