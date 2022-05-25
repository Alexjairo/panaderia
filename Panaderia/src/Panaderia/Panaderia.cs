using PanaderiaModel;
using PanaderiaD;

namespace Panaderia
{



    public class PanaderiaS
    {

        PedidosCSV RepoPedidos;
        PanesPedidosCSV RepoPanPedido;
        public List<Pedido> misPedidos;
        public List<Pan> misProductos;
        public List<Panes> misPanesPorPedido;
        public PanaderiaS(PedidosCSV repoP, PanesPedidosCSV repoPanPedido)
        {

            RepoPedidos = repoP;
            RepoPanPedido = repoPanPedido;
            misPedidos = RepoPedidos.read();
            misPanesPorPedido = RepoPanPedido.read();
            hacerPan();
            asignarPanPedidoAPedido();
            actualizarAlDia();

        }
        public void hacerPan()
        {
            misProductos = new List<Pan>();
            Pan Cateto = new Pan(tipoDePan.Cateto, 2.30M);
            Pan Francesilla = new Pan(tipoDePan.Francesilla, 0.50M);
            Pan Hogaza = new Pan(tipoDePan.Hogaza, 1.20M);
            Pan Mollete = new Pan(tipoDePan.Mollete, 1.50M);
            Pan Pataqueta = new Pan(tipoDePan.Pataqueta, 0.90M);
            misProductos.Add(Cateto);
            misProductos.Add(Francesilla);
            misProductos.Add(Hogaza);
            misProductos.Add(Mollete);
            misProductos.Add(Pataqueta);
        }

 public void actualizarMisPedidosConPedidoActualizado()
        {
            RepoPedidos.save(misPedidos);

        }
        public void marcarAPagadoTodos()
        {
            foreach (Pedido i in misPedidos)
            {
                if (i.estado.ToString().Equals(estadoPedido.pendiente.ToString()))
                {
                    i.estado = estadoPedido.pagado;
                }
                else
                {
                    i.estado = i.estado;
                }
            }
            RepoPedidos.save(misPedidos);
        }
        public void cambiarFechaPedido()
        {
            foreach (Pedido i in misPedidos)
            {
                if (i.fecha.ToShortDateString().Equals(undiaMas(DateTime.Today).ToShortDateString()))
                {
                    i.fecha = i.fecha;
                }
                else
                {
                    i.fecha = undiaMas(DateTime.Today);
                }
            }
            RepoPedidos.save(misPedidos);
        }
        public Decimal calcularPrecioPedido(Dictionary<Pan, int> unaLista)
        {
            Decimal devolver = 0;
            foreach (var i in unaLista)
            {
                devolver = devolver + (i.Key.precio * i.Value);
            }
            return devolver;
        }
        public void nuevoPedido(Pedido p, Dictionary<Pan, int> uno)
        {
            misPedidos.Add(p);
            RepoPedidos.save(misPedidos);
            guardarPanespedido(p, uno);
        }

        public void guardarPanespedido(Pedido p, Dictionary<Pan, int> uno)
        {
            List<Panes>nuevaLista = new List<Panes>();
            foreach (var i in uno)
            {
                Panes nuevo = new Panes
                               (
                    ID: p.ID,
                    pan: i.Key,
                    cantidad: i.Value
                );
                nuevaLista.Add(nuevo);
            }
            foreach (var i in nuevaLista)
            {
                misPanesPorPedido.Add(i);
            }
            RepoPanPedido.save(misPanesPorPedido);
            asignarPanPedidoAPedido();
        }

        public void asignarPanPedidoAPedido()
        {
            foreach (Panes x in misPanesPorPedido)
            {
                misPedidos.Find(pedido => x.ID.ToString().Equals(pedido.ID.ToString())).listaDePan.Add(x);
            }
        }
        public void borrarPedido(Pedido uno)
        {
            misPanesPorPedido.RemoveAll(pedido => uno.ID.ToString().Equals(pedido.ID.ToString()));
            misPedidos.Remove(misPedidos.Find(pedido => uno.ID.ToString().Equals(pedido.ID.ToString())));
            RepoPanPedido.save(misPanesPorPedido);
            RepoPedidos.save(misPedidos);

        }
        public void actualizarAlDia()
        {
            try
            {
                foreach (Pedido i in misPedidos)
                {
                  
                        if ((i.fecha.CompareTo(DateTime.Today) == 0) && (i.estado == estadoPedido.pendiente))
                        {
                            Deuda nueva = new Deuda
                            (
                                dniCliente: i.dniCliente,
                                fecha: i.fecha,
                                importe: i.precioPedido
                            );
                            /* misDeudas.Add(nueva);
                             RepoDeudas.guardar(misDeudas);*/
                            i.estado = estadoPedido.pagado;
                        }
                        else
                        if ((i.fecha.CompareTo(DateTime.Today) == 0) && (i.estado == estadoPedido.pagado))
                        {
                            i.fecha = undiaMas(i.fecha);
                            i.estado = estadoPedido.pendiente;
                        }
                    
                   
                        if ((i.fecha.CompareTo(DateTime.Today) == 0) && (i.estado == estadoPedido.pendiente))
                        {
                            Deuda nueva = new Deuda
                            (
                                dniCliente: i.dniCliente,
                                fecha: i.fecha,
                                importe: i.precioPedido
                            );
                            /* misDeudas.Add(nueva);
                             RepoDeudas.guardar(misDeudas);*/
                            borrarPedido(i);
                        }
                        else
                        if ((i.fecha.CompareTo(DateTime.Today) == 0) && (i.estado == estadoPedido.pagado))
                        {
                            borrarPedido(i);
                        }
                    
                }
                RepoPedidos.save(misPedidos);
            }
            catch (System.InvalidOperationException e)
            {
                Console.WriteLine(e.Message.ToString());
            }

        }
        public DateTime undiaMas(DateTime una)
      => new DateTime(una.Year, una.Month, una.Day + 1);

        public Pedido encontrarPedidoConPedido(Pedido uno) => misPedidos.Find(pedido => uno.ID.ToString().Equals(pedido.ID.ToString()));


    }
}