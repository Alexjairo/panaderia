using PanaderiaD;   
using Panaderia;
using PanaderiaConsole;


var RePoP = new PedidosCSV();
var RepoPanPedido = new PanesPedidosCSV();
var view = new VIew();
var sistema = new PanaderiaS(RePoP, RepoPanPedido);
var controlador = new Controller(view, sistema);
controlador.Run();