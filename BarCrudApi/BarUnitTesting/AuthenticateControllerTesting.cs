using BarCrudApi.Auth;
using BarCrudApi.Controllers;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using BarCrudMVC.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarUnitTesting
{
    public class AuthenticateControllerTesting
    {
        private readonly Mock<IUserManagementService> _userManagementService = new();

        [Fact]
        //Registro un usuario con rol User con exito
        public async void Register_Success()
        {
            // Arrange
            _userManagementService.Setup(repo => repo
            .Register(It.IsAny<RegisterViewModel>()))
            .ReturnsAsync(true);   

            var controller = new AuthenticateController(_userManagementService.Object);

            // Act
            var result = (OkObjectResult)await controller.Register(It.IsAny<RegisterViewModel>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _userManagementService.Verify(x => x.Register(It.IsAny<RegisterViewModel>()), Times.Once);            
        }
        [Fact]
        //El intento de registrar un usuario de rol user falla, se devuelve status 500
        public async void Register_Fail()
        {
            // Arrange
            _userManagementService.Setup(repo => repo
            .Register(It.IsAny<RegisterViewModel>()))
            .ReturnsAsync(false);

            var controller = new AuthenticateController(_userManagementService.Object);

            // Act
            var result = (ObjectResult)await controller.Register(It.IsAny<RegisterViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            _userManagementService.Verify(x => x.Register(It.IsAny<RegisterViewModel>()), Times.Once);
        }
        [Fact]
        //Modelo invalido para registrar usuario, se devuelve status 400
        public async void Register_InValidModel_StatusCode400BadRequest()
        {
            // Arrange
            _userManagementService.Setup(repo => repo
            .Register(It.IsAny<RegisterViewModel>()))
            .ReturnsAsync(true);

            var controller = new AuthenticateController(_userManagementService.Object);
            controller.ModelState.AddModelError("Nombre","Required");

            // Act
            var result = (ObjectResult)await controller.Register(It.IsAny<RegisterViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);           
        }
    }
}
