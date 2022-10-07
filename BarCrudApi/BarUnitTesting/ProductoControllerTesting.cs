using BarCrudApi.Controllers;
using BarCrudApi.Models;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.ContentModel;
using System.Collections.Generic;
using Xunit;
namespace BarUnitTesting
{
    public class ProductoControllerTesting
    {
        private readonly Mock<IProductoService> _mockProductoService = new();
              
        [Fact]
        //Se agrega un producto con exito
        public async void AddProducto_Success()
        {
            // Arrange
            _mockProductoService.Setup(repo => repo.Add(It.IsAny<ProductoAdminViewModel>()))
                .ReturnsAsync(true);
            var controller = new ProductoController(_mockProductoService.Object);

            // Act
            var result = (OkObjectResult)await controller.Add(It.IsAny<ProductoAdminViewModel>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockProductoService.Verify(x => x.Add(It.IsAny<ProductoAdminViewModel>()), Times.Once);
            //Assert.True(result.Value);
        }
        [Fact]
        //Falla el intento de agregar un producto, se devuelve status 500
        public async void AddProducto_Fail()
        {
            // Arrange
            _mockProductoService.Setup(repo => repo.Add(It.IsAny<ProductoAdminViewModel>()))
                .ReturnsAsync(false);
            var controller = new ProductoController(_mockProductoService.Object);

            // Act
            var result = (ObjectResult)await controller.Add(It.IsAny<ProductoAdminViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            _mockProductoService.Verify(x => x.Add(It.IsAny<ProductoAdminViewModel>()), Times.Once);
        }
        [Fact]
        //el modelo ingresado no es valido, se devuelve notFound
        public async void AddProducto_InValidModel_NotFound()
        {
            // Arrange
            _mockProductoService.Setup(repo => repo.Add(It.IsAny<ProductoAdminViewModel>()))
                .ReturnsAsync(true);
            var controller = new ProductoController(_mockProductoService.Object);
            controller.ModelState.AddModelError("Nombre","Required");

            // Act
            var result = (ObjectResult)await controller.Add(It.IsAny<ProductoAdminViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        [Fact]
        //Llamar a GetAllConFechaBaja devuelve una lista de ProductoAdminViewModel
        public async Task GetAllConFechaBaja_DevuelveLista()
        {
            // Arrange
            _mockProductoService.Setup(repo => repo.GetAll())
                .ReturnsAsync(new List<ProductoAdminViewModel>());
            var controller = new ProductoController(_mockProductoService.Object);

            // Act
            var result = (OkObjectResult)await controller.GetAllConFechaBaja();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProductoAdminViewModel>>(result.Value);
            _mockProductoService.Verify(x => x.GetAll(), Times.Once);
        }
        [Fact]
        //Llamar a GetAllConFechaBaja devuelve una lista de ProductoViewModel
        public async Task GetAllByBar_DevuelveLista()
        {
            // Arrange
            _mockProductoService.Setup(repo => repo.GetAllByBar(It.IsAny<int>()))
               .ReturnsAsync(new List<ProductoViewModel>());             
            var controller = new ProductoController(_mockProductoService.Object);

            // Act
            var result = (OkObjectResult)await controller.GetbyBar(It.IsAny<int>()); ;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<ProductoViewModel>>(result.Value);
            _mockProductoService.Verify(x => x.GetAllByBar(It.IsAny<int>()), Times.Once);
        }     
    }
}