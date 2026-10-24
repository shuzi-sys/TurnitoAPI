using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TurnitoAPI.Data;
using TurnitoAPI.Dtos.Appointment;
using TurnitoAPI.Services.Interfaces;

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

        [EnableRateLimiting("read")]
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

        [EnableRateLimiting("read")]
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

        [EnableRateLimiting("write")]
        [HttpPost("appointment")]
        public async Task<IActionResult> CreateAppointment(Create_Appointment_Dto appointmentDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var appointment = await _appointmentService.CreateAppointment(userId, appointmentDto);
            if (appointment == null)
            {
                return BadRequest();
            }
            return Created(string.Empty, appointment);
        }

        [EnableRateLimiting("write")]
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

        [EnableRateLimiting("write")]
        [HttpDelete("appointment/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _appointmentService.DeleteAppointment(userId, id);
            return NoContent();
        }
    }
}
