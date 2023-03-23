﻿// <copyright file="CategoryService.cs" company="PomodoroGroup_GL_BaseCamp">
// Copyright (c) PomodoroGroup_GL_BaseCamp. All rights reserved.
// </copyright>

using Pomodoro.Dal.Entities;
using Pomodoro.Dal.Repositories.Interfaces;
using Pomodoro.Services.Base;
using Pomodoro.Services.Models;

namespace Pomodoro.Services
{
    /// <summary>
    /// Perform operations with categories.
    /// </summary>
    public class CategoryService : BaseService<Category, CategoryModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="repository">Implementation of ICategoryRepository <see cref="ICategoryRepository"/>.</param>
        public CategoryService(ICategoryRepository repository)
            : base(repository)
        {
        }
    }
}