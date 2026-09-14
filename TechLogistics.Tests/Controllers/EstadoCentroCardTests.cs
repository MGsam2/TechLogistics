extern alias Client;

using Bunit;
using Microsoft.AspNetCore.Components;
using Xunit;

using EstadoCentroCard =
    Client::TechLogistics.Client.Components.EstadoCentroCard;

using ClientInventarioActualizacion =
    Client::TechLogistics.Grpc.InventarioActualizacion;

namespace TechLogistics.Tests.Controllers;

public class EstadoCentroCardTests : BunitContext
{
    [Fact]
    public void MuestraEstadoEnLinea_CuandoCentroEstaEnLinea()
    {
        // Arrange
        var centro = new ClientInventarioActualizacion
        {
            EnLinea = true
        };

        // Act
        var componente = Render<EstadoCentroCard>(
            parameters => parameters
                .Add(p => p.Centro, centro));

        // Assert
        Assert.Contains("En línea", componente.Markup);
        Assert.DoesNotContain("Advertencia", componente.Markup);
    }

    [Fact]
    public void MuestraAdvertencia_CuandoCentroEstaFueraDeLinea()
    {
        // Arrange
        var centro = new ClientInventarioActualizacion
        {
            EnLinea = false
        };

        // Act
        var componente = Render<EstadoCentroCard>(
            parameters => parameters
                .Add(p => p.Centro, centro));

        // Assert
        Assert.Contains("Advertencia", componente.Markup);
        Assert.DoesNotContain("En línea", componente.Markup);
    }

    [Fact]
    public void AlSeleccionarCentro_InvocaCallbackConCentroId()
    {
        // Arrange
        var centro = new ClientInventarioActualizacion
        {
            CentroId = 7,
            EnLinea = true
        };

        var centroSeleccionado = 0;

        var componente = Render<EstadoCentroCard>(
            parameters => parameters
                .Add(p => p.Centro, centro)
                .Add(
                    p => p.OnCentroSeleccionado,
                    EventCallback.Factory.Create<int>(
                        this,
                        id => centroSeleccionado = id)));

        // Act
        componente.Find("button").Click();

        // Assert
        Assert.Equal(7, centroSeleccionado);
    }
}