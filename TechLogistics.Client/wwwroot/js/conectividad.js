window.techLogisticsConnectivity = {
    iniciar: function (dotNetHelper) {

        const notificar = () => {
            dotNetHelper.invokeMethodAsync(
                "CambiarEstadoConexion",
                navigator.onLine
            );
        };

        window.addEventListener("online", notificar);
        window.addEventListener("offline", notificar);

        notificar();
    }
};