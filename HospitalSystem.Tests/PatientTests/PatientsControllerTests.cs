using HospitalSystem.Domain.Entities;
using HospitalSystem.WebApi.Controllers;
using HospitalSystem.Services.PatientService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HospitalSystem.Tests.PatientTests
{
    public class PatientsControllerTests
    {
        // List of mock patients 
        private List<Patient> GetMockPatients()
        {
            return new List<Patient>
            {
                new Patient { Id = 1, Name = "Name 1", Address = "Address 1", PhoneNumber = "+1 (111) 111-1111" },
                new Patient { Id = 2, Name = "Name 2", Address = "Address 2", PhoneNumber = "+1 (222) 222-2222" }
            };
        }

        // Paginated list of mock patients 
        private PaginatedResult<Patient> GetMockPaginatedPatients(List<Patient> patients)
        {
            return new PaginatedResult<Patient>(patients, 2, 1, 1);
        }

        // Mock patient service
        private Mock<IPatientService> CreateMockPatientService(List<Patient> patients)
        {
            var mockPatientService = new Mock<IPatientService>();

            // Setup mock patient service methods
            mockPatientService.Setup(s => s.GetAllPatientsAsync(1, 1)).ReturnsAsync(GetMockPaginatedPatients(patients));
            mockPatientService.Setup(s => s.GetPatientByIdAsync(It.IsAny<int>()))
                              .ReturnsAsync((int id) => patients.FirstOrDefault(p => p.Id == id));
            mockPatientService.Setup(s => s.CreatePatientAsync(It.IsAny<Patient>()))
                              .ReturnsAsync((Patient patient) =>
                              {
                                  patient.Id = patients.Max(p => p.Id) + 1;
                                  patients.Add(patient);
                                  return patient.Id;
                              });
            mockPatientService.Setup(s => s.UpdatePatientAsync(It.IsAny<Patient>()))
                              .ReturnsAsync((Patient patient) =>
                              {
                                  var existingPatient = patients.FirstOrDefault(p => p.Id == patient.Id);
                                  if (existingPatient != null)
                                  {
                                      existingPatient.Name = patient.Name;
                                      existingPatient.Address = patient.Address;
                                      existingPatient.PhoneNumber = patient.PhoneNumber;
                                      return true;
                                  }
                                  return false;
                              });
            mockPatientService.Setup(s => s.DeletePatientAsync(It.IsAny<int>()))
                              .ReturnsAsync((int id) =>
                              {
                                  var existingPatient = patients.FirstOrDefault(p => p.Id == id);
                                  if (existingPatient != null)
                                  {
                                      patients.Remove(existingPatient);
                                      return true;
                                  }
                                  return false;
                              });

            return mockPatientService;
        }

        [Fact]
        public async Task GetAllPatients_ReturnsOkResult()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            
            // Act
            var result = await controller.GetAllPatients(1, 1);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var model = okResult.Value as PaginatedResult<Patient>;
            Assert.Equal(2, model.Data.Count);
        }

        [Fact]
        public async Task GetPatientById_WithExistingId_ReturnsPatient()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var expectedPatient = patients[0];

            // Act
            var result = await controller.GetPatientById(expectedPatient.Id);

            // Assert
            var okResult = Assert.IsType<ActionResult<Patient>>(result);
            var model = Assert.IsAssignableFrom<ActionResult<Patient>>(okResult);
            Assert.Equal(expectedPatient.Id, model.Value.Id);
        }
        
        [Fact]
        public async Task GetPatientById_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var nonExistingId = 100;

            // Act
            var result = await controller.GetPatientById(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreatePatient_ValidPatient_ReturnsCreatedAtAction()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var newPatient = new Patient { Name = "Name 3", Address = "Address 3", PhoneNumber = "+1 (333) 333-3333" };

            // Act
            var result = await controller.CreatePatient(newPatient);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdPatient = Assert.IsType<Patient>(createdAtActionResult.Value);
            Assert.Equal(newPatient.Name, createdPatient.Name);
            Assert.Equal(newPatient.Address, createdPatient.Address);
            Assert.Equal(newPatient.PhoneNumber, createdPatient.PhoneNumber);
        }

        [Fact]
        public async Task UpdatePatient_WithExistingIdAndValidData_ReturnsNoContent()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var patientToUpdate = patients[0];
            var updatedData = new Patient { Id = patientToUpdate.Id, Name = "Updated Name 1", Address = "Updated Address 1", PhoneNumber = "+1 (222) 111-1111" };

            // Act
            var result = await controller.UpdatePatient(patientToUpdate.Id, updatedData);

            // Assert
            Assert.IsType<NoContentResult>(result);
            var updatedPatient = patients.FirstOrDefault(p => p.Id == patientToUpdate.Id);
            Assert.NotNull(updatedPatient);
            Assert.Equal(updatedData.Name, updatedPatient.Name);
            Assert.Equal(updatedData.Address, updatedPatient.Address);
            Assert.Equal(updatedData.PhoneNumber, updatedPatient.PhoneNumber);
        }

        [Fact]
        public async Task UpdatePatient_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var nonExistingId = 100;
            var updatedData = new Patient { Id = nonExistingId, Name = "Updated Name 3", Address = "Updated Address 3", PhoneNumber = "+1 (333) 222-2222" };

            // Act
            var result = await controller.UpdatePatient(nonExistingId, updatedData);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePatient_WithExistingId_ReturnsNoContent()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var patientToDelete = patients[0];

            // Act
            var result = await controller.DeletePatient(patientToDelete.Id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePatient_WithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var patients = GetMockPatients();
            var controller = new PatientsController(CreateMockPatientService(patients).Object);
            var nonExistingId = 100;

            // Act
            var result = await controller.DeletePatient(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}