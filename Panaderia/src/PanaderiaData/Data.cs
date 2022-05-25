using PanaderiaModel;
using System.Globalization;

namespace PanaderiaD

{
   
        public class PedidosCSV : IData<Pedido>
        {
            string _fileP = "../csv/productos.csv";

            public void save(List<Pedido> misPedidos)
            {
                List<string> data = new() { };
                misPedidos.ForEach(Pedido =>
                {
                    var str = $"{Pedido.ID},{Pedido.dniCliente},{Pedido.fecha.ToShortDateString()},{Pedido.precioPedido.ToString(CultureInfo.InvariantCulture)},{Pedido.estado}";
                    data.Add(str);
                });
                File.WriteAllLines(_fileP, data);

            }

            public List<Pedido> read()
            {
                List<Pedido> misPedidos = new();
                var data = File.ReadAllLines(_fileP).Where(row => row.Length > 0).ToList();
                data.ForEach(row =>
                {
                    var campos = row.Split(",");
                    Pedido pedido = new Pedido
                    (
                        ID: Guid.Parse(campos[0]),
                        dniCliente: campos[1],
                        fecha: DateTime.Parse(campos[2]),
                        precioPedido: Decimal.Parse(campos[3], CultureInfo.InvariantCulture),
                        estado: (estadoPedido)Enum.Parse((typeof(estadoPedido)), campos[4])
                    );
                    misPedidos.Add(pedido);
                });

                return misPedidos;

            }
        }


        public class PanesPedidosCSV : IData<Panes>
        {
            string _filePanesPedidos = "../csv/panesBodega.csv";

            public void save(List<Panes> misPanesPorPedido)
            {
                List<string> data = new() { };
                misPanesPorPedido.ForEach(PanesPedido =>
                {
                    var str = $"{PanesPedido.ID.ToString()},{PanesPedido.pan.ToCSV()},{PanesPedido.cantidad.ToString()}";
                    data.Add(str);
                });
                File.WriteAllLines(_filePanesPedidos, data);

            }

            public List<Panes> read()
            {
                List<Panes> misPanesPorPedido = new();
                var data = File.ReadAllLines(_filePanesPedidos).Where(row => row.Length > 0).ToList();
                data.ForEach(row =>
                {
                    var campos = row.Split(",");
                    var panesPedido = new Panes
                    (
                        ID: Guid.Parse(campos[0]),
                        pan: new Pan((tipoDePan)Enum.Parse((typeof(tipoDePan)), campos[1]), Decimal.Parse(campos[2], CultureInfo.InvariantCulture)),
                        cantidad: int.Parse(campos[3])
                    );
                    misPanesPorPedido.Add(panesPedido);
                });

                return misPanesPorPedido;

            }

        }

    
}