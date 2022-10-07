using BarCrudApi.Controllers;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
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
    public class PedidoControllerTesting
    {
        private readonly Mock<IPedidoService> _mockPedidoService = new();

        [Fact]
        //Traigo el viewModel del pedido pendiente de un usuario,
        public async void GetPedidoPendiente_Success()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .GetPedidoPendiente(It.IsAny<String>()))
            .ReturnsAsync(new Pedido());

            _mockPedidoService.Setup(repo => repo
            .VerificarStock(It.IsAny<Pedido>()))
            .Returns(true);

            var controller = new PedidoController(_mockPedidoService.Object);

            // Act
            var result = (OkObjectResult)await controller.GetPedidoPendiente(It.IsAny<string>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<PedidoViewModel>(result.Value);
            _mockPedidoService.Verify(x => x.GetPedidoPendiente(It.IsAny<string>()), Times.Once);
            _mockPedidoService.Verify(x => x.VerificarStock(It.IsAny<Pedido>()), Times.Once);
        }
        [Fact]
        //si el pedido pendiente existe, pero el stock de los productos
        //no puede satisface el pedido, se devuelve status 409
        public async void GetPedidoPendiente_SinStock_Status409Conflict()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .GetPedidoPendiente(It.IsAny<String>()))
            .ReturnsAsync(new Pedido());

            _mockPedidoService.Setup(repo => repo
            .VerificarStock(It.IsAny<Pedido>()))
            .Returns(false);

            var controller = new PedidoController(_mockPedidoService.Object);

            // Act
            var result = (ObjectResult)await controller.GetPedidoPendiente(It.IsAny<string>());

            // Assert
            Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
            _mockPedidoService.Verify(x => x.GetPedidoPendiente(It.IsAny<string>()), Times.Once);
            _mockPedidoService.Verify(x => x.VerificarStock(It.IsAny<Pedido>()), Times.Once);
        }
        [Fact]
        //No se encontro el pedido
        public async void GetPedidoPendiente_Status404NotFound()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .GetPedidoPendiente(It.IsAny<String>()));

            var controller = new PedidoController(_mockPedidoService.Object);

            // Act
            var result = (ObjectResult)await controller.GetPedidoPendiente(It.IsAny<string>());

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            _mockPedidoService.Verify(x => x.GetPedidoPendiente(It.IsAny<string>()), Times.Once);
        }
        [Fact]
        //Se remueve con exito un producto del pedido
        public async void RemoveProducto_Sucess()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .RemoveProducto(It.IsAny<CarritoRemoveViewModel>()))
            .ReturnsAsync(true);

            var controller = new PedidoController(_mockPedidoService.Object);

            // Act
            var result = (OkObjectResult)await controller.RemoveProducto(It.IsAny<CarritoRemoveViewModel>());

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _mockPedidoService.Verify(x => x.RemoveProducto(It.IsAny<CarritoRemoveViewModel>()), Times.Once);
        }
        [Fact]
        //Modelo invalido en RemoveProducto se devuelve notFound
        public async void RemoveProducto_InValidModel_NotFound()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .RemoveProducto(It.IsAny<CarritoRemoveViewModel>()))
            .ReturnsAsync(true);

            var controller = new PedidoController(_mockPedidoService.Object);
            controller.ModelState.AddModelError("UserId", "Required");

            // Act
            var result = (ObjectResult)await controller.RemoveProducto(It.IsAny<CarritoRemoveViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
        [Fact]
        //Falla el intento de remover un producto, se devuelve statusCode 500
        public async void RemoveProducto_Fail_StatusCode500()
        {
            // Arrange
            _mockPedidoService.Setup(repo => repo
            .RemoveProducto(It.IsAny<CarritoRemoveViewModel>()))
            .ReturnsAsync(false);

            var controller = new PedidoController(_mockPedidoService.Object);

            // Act
            var result = (ObjectResult)await controller.RemoveProducto(It.IsAny<CarritoRemoveViewModel>());

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            _mockPedidoService.Verify(x => x.RemoveProducto(It.IsAny<CarritoRemoveViewModel>()), Times.Once);
        }
    }
}
