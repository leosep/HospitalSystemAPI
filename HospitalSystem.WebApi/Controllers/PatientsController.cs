using HospitalSystem.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using HospitalSystem.Services.PatientService.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace HospitalSystem.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Gets a paginated list of all patients.
        /// </summary>
        /// /// <param name="page">The ID of the patient.</param>
        /// /// <param name="pageSize">The ID of the patient.</param>
        /// <returns>A paginated list of patients.</returns>
        [HttpGet("{page}/{pageSize}")]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Patient>))]
        public async Task<ActionResult<PaginatedResult<Patient>>> GetAllPatients(int page = 1, int pageSize = 10)
        {
            var patients = await _patientService.GetAllPatientsAsync(page, pageSize);
            return Ok(patients);
        }

        /// <summary>
        /// Gets a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient.</param>
        /// <returns>The patient with the specified ID.</returns>
        [HttpGet("{id}")]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        /// <summary>
        /// Adds a new patient.
        /// </summary>
        /// <param name="patient">The patient to add.</param>
        /// <returns>The added patient with its assigned ID.</returns>
        [HttpPost]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> CreatePatient(Patient patient)
        {
            var createdPatientId = await _patientService.CreatePatientAsync(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = createdPatientId }, patient);
        }

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="id">The ID of the patient to update.</param>
        /// <param name="updatedPatient">The updated patient data.</param>
        /// <returns>The updated patient.</returns>
        [HttpPut("{id}")]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Patient))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdatePatient(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }

            var result = await _patientService.UpdatePatientAsync(patient);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a patient by ID.
        /// </summary>
        /// <param name="id">The ID of the patient to delete.</param>
        [HttpDelete("{id}")]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
