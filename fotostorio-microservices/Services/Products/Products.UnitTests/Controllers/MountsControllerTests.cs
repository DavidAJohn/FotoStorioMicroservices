using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Products.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.UnitTests.Controllers;
public class MountsControllerTests : ControllerBase
{
    private readonly ILogger<MountsController> _logger;
    private readonly IMountRepository _mountRepository;

    public MountsControllerTests()
    {
        _mountRepository = Substitute.For<IMountRepository>();
        _logger = Substitute.For<ILogger<MountsController>>();
    }

    [Fact]
    public async Task GetMounts_ShouldReturnListOfMounts_WhenCalled()
    {
        // Arrange
        var mounts = new List<Mount>
        {
            new Mount { Id = 1, Name = "Mount 1" },
            new Mount { Id = 2, Name = "Mount 2" }
        };

        _mountRepository.ListAllAsync().Returns(mounts);

        var controller = new MountsController(_logger, _mountRepository);

        // Act
        var result = (OkObjectResult)await controller.GetMounts();

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<List<Mount>>();
        result.Value.Should().BeEquivalentTo(mounts);
    }

    [Fact]
    public async Task GetMountById_ShouldReturnMount_WhenMountIdExists()
    {
        // Arrange
        var mount = new Mount { Id = 1, Name = "Mount 1" };

        _mountRepository.GetByIdAsync(mount.Id).Returns(mount);

        var sut = new MountsController(_logger, _mountRepository);

        // Act
        var result = (OkObjectResult)await sut.GetMountById(mount.Id);

        // Assert
        result.StatusCode.Should().Be(200);
        result.Value.Should().BeOfType<Mount>();
        result.Value.Should().BeEquivalentTo(mount);
    }

    [Fact]
    public async Task GetMountById_ShouldReturnNotFound_WhenMountIdDoesNotExist()
    {
        // Arrange
        _mountRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        var sut = new MountsController(_logger, _mountRepository);

        // Act
        var result = (NotFoundResult)await sut.GetMountById(int.MinValue);

        // Assert
        result.StatusCode.Should().Be(404);
    }
}
