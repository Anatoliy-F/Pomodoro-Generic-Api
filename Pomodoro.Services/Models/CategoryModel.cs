﻿// <copyright file="CategoryModel.cs" company="PomodoroGroup_GL_BaseCamp">
// Copyright (c) PomodoroGroup_GL_BaseCamp. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Pomodoro.Dal.Configs;
using Pomodoro.Dal.Entities;
using Pomodoro.Services.Models.Interfaces;

namespace Pomodoro.Services.Models
{
    /// <summary>
    /// Represent category for client.
    /// </summary>
    public class CategoryModel : IBaseModel<Category>
    {
        /// <summary>
        /// Gets or sets category id.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets category name.
        /// </summary>
        [Required(ErrorMessage = "Category {0} is required.")]
        [StringLength(
            PomoConstants.CategoryNameMaxLength,
            ErrorMessage = "The {0} should be less than {1} characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets category description.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [StringLength(
            PomoConstants.CategoryDescriptionMaxLength,
            ErrorMessage = "The {0} should be less or equal than {1} characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets foreign key to AppUser Entity.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid AppUserId { get; set; }

        /// <summary>
        /// Gets or sets collection of schedules related to this category.
        /// </summary>
        public ICollection<ScheduleModel> Schedules { get; set; } = new List<ScheduleModel>();

        /// <summary>
        /// Gets or sets collection of tasks related to this category.
        /// </summary>
        public ICollection<TaskModel> Tasks { get; set; } = new List<TaskModel>();

        /// <inheritdoc/>
        public void Assign(Category entity, bool isMapOwner = true)
        {
            this.Id = entity.Id;
            this.Name = entity.Name;
            this.Description = entity.Description;
            this.AppUserId = isMapOwner ? entity.AppUserId : Guid.Empty;

            this.Tasks = this.Tasks.Any() ? new List<TaskModel>() : this.Tasks;
            foreach (var task in entity.Tasks)
            {
                var model = new TaskModel();
                model.Assign(task);
                this.Tasks.Add(model);
            }

            this.Schedules = this.Schedules.Any() ? new List<ScheduleModel>() : this.Schedules;
            foreach (var schedule in entity.Schedules)
            {
                var model = new ScheduleModel();
                model.Assign(schedule);
                this.Schedules.Add(model);
            }
        }

        /// <inheritdoc/>
        public Category ToDalEntity(Guid userId)
        {
            return new Category
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                AppUserId = userId,
                Tasks = this.Tasks.Any() ?
                    this.Tasks.Select(e => e.ToDalEntity(userId)).ToList() : new List<AppTask>(),
                Schedules = this.Schedules.Any() ?
                    this.Schedules.Select(e => e.ToDalEntity(userId)).ToList() : new List<Schedule>(),
            };
        }
    }
}
