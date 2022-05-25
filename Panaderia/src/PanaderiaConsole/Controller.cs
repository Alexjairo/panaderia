using PanaderiaModel;
using Panaderia;
using System.Globalization;

namespace PanaderiaConsole
{
    public class Controller
    {
        private VIew vista;
        private PanaderiaS sistema;
        private Dictionary<string, Action> panel;
        private Dictionary<string, Action> pedidos;
        private Dictionary<string, Action> marcarP;
        private Dictionary<string, Action> validarP;



        public Controller(VIew vista, PanaderiaS logicaNegocio)
        {
            this.vista = vista;
            sistema = logicaNegocio;
            panel = new Dictionary<string, Action>()
            {
                {"Opciónes de Pedido",gestionPedidos},
                {"Salir ",salir}
            };
        }

        public void Run()
        {
            vista.LimpiarPantalla();
            var menu = panel.Keys.ToList<string>();

            while (true)
                try
                {
                    vista.LimpiarPantalla();
                    var key = vista.TryObtenerElementoDeLista("Panadería ", menu, "Selecciona una opción ");
                   vista.Mostrar("");
                    panel[key].Invoke();
                    vista.MostrarYReturn("Pulsa <Intro> para continuar");
                }
                catch { return; }
        }
        public void salir()
        {
            var key = "fin";
            vista.Mostrar("Gracias\n\nHasta la próxima!!\n\n");

            panel[key].Invoke();
        }
        public void volverAtras() { }



        private void gestionPedidos()
        {
            pedidos = new Dictionary<string, Action>()
            {
                {"Ver Pedidos",verPedidos},
                 {"Añadir Pedido",aniadirPedido},
                {"Marcar pedido como pagado",marcarPedidoDiaSiguiente},
                {"Cambiar Pedido",cambiarPedido},
                {"Borrar Pedido",borrarPedidio},
                {"Volver atras",volverAtras}
            };
            var menuPedidos = pedidos.Keys.ToList<string>();
            try
            {
                vista.LimpiarPantalla();
                var key = vista.TryObtenerElementoDeLista("OPciónes", menuPedidos, "Selecciona una opción ");
                vista.Mostrar("");
                pedidos[key].Invoke();

            }
            catch { return; }
        }
        private void verPedidos()
        {
            foreach (Pedido i in sistema.misPedidos)
            {
                vista.Mostrar(i.ToString());
                System.Collections.IList list = i.listaDePan;
                for (int i1 = 0; i1 < list.Count; i1++)
                {
                    Panes j = (Panes)list[i1];
                    vista.Mostrar("" + j.ToString());

                }
            }
            vista.Mostrar("\n");
        }


        private void aniadirPedido()
        {
            try
            {
                var dniCli = vista.TryObtenerDatoDeTipo<string>("Introduzca dni");


                Dictionary<Pan, int> panParaLista = new Dictionary<Pan, int>();
                Pan panNuevo;
                int cantidad;
                string fuera = "";
                while (true)
                {
                    vista.LimpiarPantalla();
                    try
                    {
                        panNuevo = vista.TryObtenerElementoDeLista("Tipos de Pan", sistema.misProductos, "Seleciona un Pan");
                        cantidad = vista.TryObtenerDatoDeTipo<int>("Introduzca cantidad de unidades del pan seleccionado");
                        panParaLista.Add(panNuevo, cantidad);
                    }
                    catch { vista.Mostrar("\nYa se ha introducido datos para este tipo de pan\n"); }
                    fuera = vista.TryObtenerDatoDeTipo<string>("HAs terminado? ( N/S )");
                    if (fuera.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }


                var ID = Guid.NewGuid();
                var fecha = sistema.undiaMas(DateTime.Today);
                var precio = sistema.calcularPrecioPedido(panParaLista);
                var estado = estadoPedido.pendiente;
                Pedido nuevo = new Pedido
                (
                    ID: ID,
                    dniCliente: dniCli,
                    fecha: fecha.Date,
                    precioPedido: precio,
                    estado: estado
                );
                sistema.nuevoPedido(nuevo, panParaLista);
                vista.Mostrar("\n\nNuevo pedido registrado.\n", ConsoleColor.DarkYellow);

            }
            catch { return; }
        }

        public void marcarPedidoDiaSiguiente()
        {
            marcarP = new Dictionary<string, Action>()
            {
                {"Marcar un pedido  como pagado",marcarPedidoPagado},
                {"Marcar todos los pedidos como pagados",marcarAPagadoTodos},
                {"Volver ",volverAtras}
            };
            var menuMarcar = marcarP.Keys.ToList<String>();
            try
            {
                vista.LimpiarPantalla();
                var key = vista.TryObtenerElementoDeLista("Opciones para Pedido", menuMarcar, "Selecciona una opción ");
                vista.Mostrar("");
                marcarP[key].Invoke();

            }
            catch { return; }
        }

        public void marcarPedidoPagado()
        {
            Pedido nuevo = vista.TryObtenerElementoDeLista("Lista de Pedidos", sistema.misPedidos, "Seleciona una pedido");
            if (nuevo.estado.ToString().Equals(estadoPedido.pagado.ToString()))
            {
                vista.Mostrar("\nEste pedido ya esta pagado\nPorfavor, selecciona otro", ConsoleColor.Red);
            }
            else
            {
                nuevo.estado = estadoPedido.pagado;
                vista.Mostrar("\n\nPedido actualizado.\n", ConsoleColor.DarkYellow);
                sistema.actualizarMisPedidosConPedidoActualizado();
            }

        }
        public void marcarAPagadoTodos()
        {
            sistema.marcarAPagadoTodos();
            vista.Mostrar("\n\nPedidos actualizados. \n", ConsoleColor.DarkYellow);
        }

       
        public void cambiarFechaPedido()
        {
            Pedido nuevo = vista.TryObtenerElementoDeLista("Lista de Pedidos", sistema.misPedidos, "Seleciona una pedido");
            if (nuevo.fecha.ToShortDateString().Equals(sistema.undiaMas(DateTime.Today).ToShortDateString()))
            {
                vista.Mostrar("Porfavor, selecciona otro");
            }
            else
            {
                nuevo.fecha = sistema.undiaMas(DateTime.Today);
                sistema.actualizarMisPedidosConPedidoActualizado();
                vista.Mostrar("\n\nPedido actualizado para el dia siguiente.\n");
            }


        }
        public void cambiarFechasPedidos()
        {
            sistema.cambiarFechaPedido();
            vista.Mostrar("\n\nPedidos actualizado.\n");
        }


        private void cambiarPedido()
        {
            try
            {

                Pedido nuevo = vista.TryObtenerElementoDeLista<Pedido>("Lista de Pedidos", sistema.misPedidos, "Selecciona un pedido:");
                Dictionary<Pan, int> panParaLista = new Dictionary<Pan, int>();
                Pan panNuevo;
                int cantidad;
                string fuera = "";
                while (true)
                {
                    vista.LimpiarPantalla();
                    try
                    {
                        panNuevo = vista.TryObtenerElementoDeLista("Tipos de Pan", sistema.misProductos, "Seleciona un Pan");
                        cantidad = vista.TryObtenerDatoDeTipo<int>("Introduzca cantidad del pan seleccionado");
                        panParaLista.Add(panNuevo, cantidad);
                    }
                    catch { vista.Mostrar("\nYa se ha comprado el pan\n", ConsoleColor.Red); }
                    fuera = vista.TryObtenerDatoDeTipo<string>("Deseas comprar mas Pan ( S/N )");
                    if (fuera.Equals("s", StringComparison.InvariantCultureIgnoreCase))
                        break;
                }


                var ID = Guid.NewGuid();
                var fecha = sistema.undiaMas(DateTime.Today);
                var precio = sistema.calcularPrecioPedido(panParaLista);
                var estado = estadoPedido.pendiente;
                Pedido otro = new Pedido
                    (
                        ID: ID,
                        dniCliente: nuevo.dniCliente,
                        fecha: fecha.Date,
                        precioPedido: precio,
                        estado: estado
                    );

                sistema.nuevoPedido(otro, panParaLista);
                sistema.borrarPedido(nuevo);

                vista.Mostrar("\n\nPedido actualizado\n", ConsoleColor.DarkYellow);

            }
            catch { return; }
        }
        private void borrarPedidio()
        {
            Pedido nuevo = vista.TryObtenerElementoDeLista<Pedido>("Pedidos registrados", sistema.misPedidos, "Selecciona un pedido");
            sistema.borrarPedido(nuevo);
            vista.Mostrar("\n\nPedido borrado\n", ConsoleColor.DarkYellow);
        }





    }
}