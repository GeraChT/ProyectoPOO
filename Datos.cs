using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Final_POO
{
	// Delegado
	public delegate void EnviarCorreoEventHandler(string cliente, string factura, double cantidad);
	class Datos
	{
		// Evento
		public event EnviarCorreoEventHandler EnviarCorreo;

		private string _strCliente;

		public string Cliente
		{
			get { return _strCliente; }
			set { _strCliente = value; }
		}
		private string _strNFactura;

		public string NFactura
		{
			get { return _strNFactura; }
			set { _strNFactura = value; }
		}
		private double _dblCosto;

		public double Costo
		{
			get { return _dblCosto; }
			set { _dblCosto = value; }
		}
		private string _strEstado;

		public string Estado
		{
			get { return _strEstado; }
			set
			{
				if (value == "Pagado")
				{
					_strEstado = value;
					EnviarCorreo(_strCliente, _strNFactura, _dblCosto);
				}
				else _strEstado = value;
			}
		}
	}
}