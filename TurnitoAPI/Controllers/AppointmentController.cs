using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Services.Interfaces;
using System.Security.Claims;

namespace TurnitoAPI.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IappointmentService _appointmentService;
        public AppointmentController( IappointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet("company/{id:int}/appointments")]
        public async Task<IActionResult> GetAllAppointmentsForCompany(int id) {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointments = await _appointmentService.GetAllAppointmentsForCompany(userId, id);
            if (appointments == null)
            {
                return NotFound();
            }
            return Ok(appointments);
        }

        [HttpGet("appointment/{id:int}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentService.GetAppointmentById(userId, id);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        // POST api/<AppointmentController>
        [HttpPost("appointment")]
        public async Task<IActionResult> CreateAppointment(Create_Appointment_Dto appointmentDto)
        {
            var appointment = await _appointmentService.CreateAppointment(appointmentDto);
            if (appointment == null)
            {
                return BadRequest();
            }
            return Created(string.Empty, appointment);
        }

        [HttpPatch("appointment/{id:int}")]
        public async Task<IActionResult> UpdateAppointment(int id, Update_Appointment_Dto appointmentDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentService.UpdateAppointment(userId, id, appointmentDto);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        // DELETE api/<AppointmentController>/5
        [HttpDelete("appointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _appointmentService.DeleteAppointment(userId, id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
