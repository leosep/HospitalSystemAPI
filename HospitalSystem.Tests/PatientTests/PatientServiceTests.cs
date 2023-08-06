using HospitalSystem.DataAccess.Interfaces;
using HospitalSystem.Domain.Entities;
using HospitalSystem.Services.PatientService;
using Moq;
using Xunit;

namespace HospitalSystem.Tests.PatientTests
{
    public class PatientServiceTests
    {
        // List of mock patients for testing
        private List<Patient> GetMockPatients()
        {
            return new List<Patient>
            {
                new Patient { Id = 1, Name = "Name 1", Address = "Address 1", PhoneNumber = "+1 (111) 111-1111" },
                new Patient { Id = 2, Name = "Name 2", Address = "Address 2", PhoneNumber = "+1 (222) 222-2222" }
            };
        }

        // Paginated list of mock patients for testing
        private PaginatedResult<Patient> GetMockPaginatedPatients(List<Patient> patients)
        {
            return new PaginatedResult<Patient>(patients, 2, 1, 1);
        }

        // Mock patient service
        private Mock<IPatientRepository> CreateMockPatientRepository(List<Patient> patients)
        {
            var newPatient = new Patient { Name = "Name 3", Address = "Address 3", PhoneNumber = "+1 (333) 333-3333" };
            var newPatientId = 3;
            var nonExistingPatient = new Patient { Id = 999, Name = "Non Existing", Address = "Updated Adress 999", PhoneNumber = "+1 (999) 111-1111" };
            var nonExistingPatientId = 999;

            // Setup mock patient service methods
            var mockPatientRepository = new Mock<IPatientRepository>();

            mockPatientRepository.Setup(r => r.GetAllPatientsAsync(1, 1)).ReturnsAsync(GetMockPaginatedPatients(patients));

            if(patients.Any()) mockPatientRepository.Setup(r => r.GetPatientByIdAsync(patients[0].Id))
                .ReturnsAsync(patients[0]);

            mockPatientRepository.Setup(r => r.GetPatientByIdAsync(999))
                .ReturnsAsync((Patient)null);

            mockPatientRepository.Setup(r => r.CreatePatientAsync(It.IsAny<Patient>()))
                .ReturnsAsync(newPatientId);

            if (patients.Any()) mockPatientRepository.Setup(r => r.UpdatePatientAsync(patients[0]))
                .ReturnsAsync(true);

            mockPatientRepository.Setup(r => r.UpdatePatientAsync(nonExistingPatient))
                .ReturnsAsync(false);

            if (patients.Any()) mockPatientRepository.Setup(r => r.DeletePatientAsync(patients[0].Id))
                .ReturnsAsync(true);

            var patientRepositoryMock = new Mock<IPatientRepository>();
            patientRepositoryMock.Setup(r => r.DeletePatientAsync(nonExistingPatientId))
                .ReturnsAsync(false);

            return mockPatientRepository;
        }

        [Fact]
        public async Task GetAllPatientsAsync_ReturnsPatients()
        {
            // Arrange
            var patients = GetMockPatients();
            var service = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var result = await service.GetAllPatientsAsync(1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(2, result.TotalRecords);
            Assert.Equal(1, result.Page);
            Assert.Equal(1, result.PageSize);
        }

        [Fact]
        public async Task GetPatientByIdAsync_ExistingId_ReturnsPatient()
        {
            // Arrange
            var patients = GetMockPatients();
            var patientId = 1;
            var expectedPatient = new Patient { Id = 1, Name = "Name 1", Address = "Address 1", PhoneNumber = "+1 (111) 111-1111" };
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var patient = await patientService.GetPatientByIdAsync(patientId);

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(expectedPatient.Id, patient.Id);
            Assert.Equal(expectedPatient.Name, patient.Name);
            Assert.Equal(expectedPatient.Address, patient.Address);
            Assert.Equal(expectedPatient.PhoneNumber, patient.PhoneNumber);
        }

        [Fact]
        public async Task GetPatientByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var patients = GetMockPatients();
            var patientId = 999;
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var patient = await patientService.GetPatientByIdAsync(patientId);

            // Assert
            Assert.Null(patient);
        }

        [Fact]
        public async Task CreatePatientAsync_ValidPatient_ReturnsNewPatientId()
        {
            // Arrange
            var expectedPatientId = 3;
            var newPatient = new Patient { Name = "Name 3", Address = "Address 3", PhoneNumber = "+1 (333) 333-3333" };

            var patientService = new PatientService(CreateMockPatientRepository(new List<Patient>()).Object);

            // Act
            var createdPatientId = await patientService.CreatePatientAsync(newPatient);

            // Assert
            Assert.Equal(expectedPatientId, createdPatientId);
        }

        [Fact]
        public async Task UpdatePatientAsync_ExistingPatient_ReturnsTrue()
        {
            // Arrange
            var patients = GetMockPatients();
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var result = await patientService.UpdatePatientAsync(patients[0]);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdatePatientAsync_NonExistingPatient_ReturnsFalse()
        {
            // Arrange
            var patients = GetMockPatients();
            var nonExistingPatient = new Patient { Id = 999, Name = "Non Existing", Address = "Updated Adress 999", PhoneNumber = "+1 (999) 111-1111" };
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var result = await patientService.UpdatePatientAsync(nonExistingPatient);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeletePatientAsync_ExistingPatient_ReturnsTrue()
        {
            // Arrange
            var patients = GetMockPatients();
            var existingPatientId = 1;
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var result = await patientService.DeletePatientAsync(existingPatientId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeletePatientAsync_NonExistingPatient_ReturnsFalse()
        {
            // Arrange
            var patients = GetMockPatients();
            var nonExistingPatientId = 999;
            var patientService = new PatientService(CreateMockPatientRepository(patients).Object);

            // Act
            var result = await patientService.DeletePatientAsync(nonExistingPatientId);

            // Assert
            Assert.False(result);
        }
    }
}