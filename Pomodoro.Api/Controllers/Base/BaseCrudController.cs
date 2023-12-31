﻿// <copyright file="BaseCrudController.cs" company="PomodoroGroup_GL_BaseCamp">
// Copyright (c) PomodoroGroup_GL_BaseCamp. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Pomodoro.Dal.Entities.Base;
using Pomodoro.Dal.Repositories.Base;
using Pomodoro.Services.Base;
using Pomodoro.Services.Models.Interfaces;
using Pomodoro.Services.Models.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace Pomodoro.Api.Controllers.Base
{
    /// <summary>
    /// Extended BaseController <see cref="BaseController"/>
    /// by adding actions for base CRUD operations.
    /// </summary>
    /// <typeparam name="TS">Service object.</typeparam>
    /// <typeparam name="TE">Entity type.</typeparam>
    /// <typeparam name="TM">DTO type.</typeparam>
    /// <typeparam name="TR">Repository type, used by service.</typeparam>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(401, "This endpoints available only for registered users")]
    public abstract class BaseCrudController<TS, TE, TM, TR> : BaseController
        where TS : IBaseService<TE, TM, TR>
        where TE : IBelongEntity, new()
        where TM : IBaseModel<TE>, new()
        where TR : IBelongRepository<TE>
    {
        /// <summary>
        /// Service for business logic.
        /// </summary>
        private readonly TS service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseCrudController{TS, TE, TM, TR}"/> class.
        /// </summary>
        /// <param name="service">Service implemented business logic.</param>
        protected BaseCrudController(TS service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets instance of service.
        /// </summary>
        protected TS Service => this.service;

        /// <summary>
        /// Return all objects belonging to user.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("own")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Retrieved all user's objects.")]
        public async Task<ActionResult<ICollection<TM>>> GetOwnAll()
        {
            return this.Ok(await this.service.GetOwnAllAsync(this.UserId));
        }

        /// <summary>
        /// Return belonging object by id, or 404 if not exist, or 403 if access denied.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("own/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerResponse(200, "The execution was successful")]
        [SwaggerResponse(403, "This object don't belong to current user")]
        [SwaggerResponse(404, "Object not found")]
        public async Task<ActionResult<TM>> GetById(Guid id)
        {
            var result = await this.service.GetOwnByIdAsync(id, this.UserId);

            return this.MapServiceResponse(result);
        }

        /// <summary>
        /// Persist new belonging to user object.
        /// </summary>
        /// <param name="model">New object.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(201, "Object created")]
        [SwaggerResponse(400, "The request was invalid")]
        public virtual async Task<ActionResult<TM>> AddOne([FromBody]TM model)
        {
            var result = await this.service.AddOneOwnAsync(model, this.UserId);
            if (result.Result == ResponseType.Ok)
            {
                return this.CreatedAtAction(nameof(this.GetById), new { model.Id }, model);
            }

            return this.MapServiceResponse(result);
        }

        /// <summary>
        /// Delete belongin object by id.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("own/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(204, "Delete successfully")]
        [SwaggerResponse(400, "No object with such id for this user")]
        public async Task<ActionResult> DeleteOne(Guid id)
        {
            var result = await this.service.DeleteOneOwnAsync(id, this.UserId);
            return this.MapServiceResponse(result);
        }

        /// <summary>
        /// Update existing object.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <param name="model">Exisitng object.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPut("own/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(200, "Update successfully")]
        [SwaggerResponse(400, "No schedule with such id for this user")]
        public virtual async Task<ActionResult> UpdateOne(Guid id, [FromBody]TM model)
        {
            if (id != model.Id)
            {
                return this.BadRequest();
            }

            var result = await this.service.UpdateOneOwnAsync(model, this.UserId);
            if (result.Result == ResponseType.Ok)
            {
                return this.Ok(model);
            }

            return this.MapServiceResponse(result);
        }

        /// <summary>
        /// Map service response to ActionResult.
        /// </summary>
        /// <typeparam name="TSR">Service response data type (object, collection or plain value).</typeparam>
        /// <param name="response">Service response object.</param>
        /// <returns>ActionResult corresponding to service response.</returns>
        protected ActionResult MapServiceResponse<TSR>(ServiceResponse<TSR> response)
        {
            return response.Result switch
            {
                ResponseType.Ok => this.Ok(response.Data),
                ResponseType.NoContent => this.NoContent(),
                ResponseType.NotFound => this.NotFound(),
                ResponseType.Forbid => this.Forbid(),
                ResponseType.Error => this.BadRequest(response.Message),
                ResponseType.Conflict => this.Conflict(response.Message),
                _ => this.BadRequest()
            };
        }
    }
}
