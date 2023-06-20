
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using tp1;

namespace tpfinal
{

	public class Estrategia
	{
		private int CalcularDistancia(string str1, string str2)
		{
			// using the method
			String[] strlist1 = str1.ToLower().Split(' ');
			String[] strlist2 = str2.ToLower().Split(' ');
			int distance = 1000;
			foreach (String s1 in strlist1)
			{
				foreach (String s2 in strlist2)
				{
					distance = Math.Min(distance, Utils.calculateLevenshteinDistance(s1, s2));
				}
			}

			return distance;
		}

        public String Consulta1(ArbolGeneral<DatoDistancia> arbol)
        {
            //Se crea una variable para almacenar el resultado
            string resultado = "";
            //Si es una hoja se retorna el texto de la misma.
            if (arbol.esHoja())
            {
                return arbol.getDatoRaiz().ToString() + " \n";
            }
            // Si no es una hoja, se procesan los hijos.
            if (arbol.getHijos() != null)
            {
                foreach (var a in arbol.getHijos())
                    //Y por medio de la recursion vamos concatenado todos los textos en la varible.
                    resultado += Consulta1(a);
            }
            return resultado;
        }

        public string Consulta2(ArbolGeneral<DatoDistancia> arbol)
        {
            /*En primer lugar, se declaran las variables que nos van a ayudar a almacenar4
              los caminos encontrados, los nodos y los caminos actuales.*/
            List<List<DatoDistancia>> caminosEncontrados = new List<List<DatoDistancia>>();
            Cola<ArbolGeneral<DatoDistancia>> cola = new Cola<ArbolGeneral<DatoDistancia>>();
            cola.encolar(arbol);

            while (cola.cantidadElementos() > 0)
            {
                ArbolGeneral<DatoDistancia> nodo = cola.desencolar();

                if (nodo.esHoja())
                {
                    List<DatoDistancia> camino = new List<DatoDistancia>();
                    ArbolGeneral<DatoDistancia> aux = nodo;

                    while (aux != null)
                    {
                        camino.Add(aux.getDatoRaiz());
                        aux = aux.getPadre();
                    }

                    camino.Reverse(); // Invertir el orden del camino
                    caminosEncontrados.Add(camino);
                }
                else
                {
                    foreach (var hijo in nodo.getHijos())
                    {
                        cola.encolar(hijo);
                    }
                }
            }
            //Por ultimo, devolvemos la cadena de texto con los caminos encontrados
            string resultado = "";
            //Agreamos un contador para enumerar los caminos.
            int n = 0;
            foreach (var camino in caminosEncontrados)
            {
                n++;
                resultado += "Camino n°"+ n +": \n";

                foreach (var ca in camino)
                {
                    resultado += ca.ToString() + "\n";
                }
                resultado+= "\n";
            }
                return resultado;
        }
        public string Consulta3(ArbolGeneral<DatoDistancia> arbol)
        {
            /*Decalaramos una variable para almacenar el resultado de la consulta y una
            cola para realizar el recorrido por niveles.*/
            string result = "";
            Cola<ArbolGeneral<DatoDistancia>> cola = new Cola<ArbolGeneral<DatoDistancia>>();

            /* Declaramos una variable aux para almacenar el nodo actual mientras 
            recorremos la cola y encolamos el arbol inicial.*/
            ArbolGeneral <DatoDistancia> aux;
            cola.encolar(arbol);

            //Ponemos el nivel en 0.
            int nivelActual = 0;

            while (cola.cantidadElementos() > 0)
            {
                /*Obtenemos la cantidad de elementos en la cola y
                    agregamos el nivel actual a la cadena de resultado.*/
                int cantNivel = cola.cantidadElementos();
                result += "Nivel: " + nivelActual + "\n";

                // Y por ultimo iteramos cada elemento del nivel actual
                for (int i = 0; i < cantNivel; i++)
                {
                    //Agregamos el texto al resultado
                    aux = cola.desencolar();
                    result += aux.getDatoRaiz().ToString() + "\n";

                    if (aux.getHijos() != null)
                    {
                        //Y si el dato actual tiene hijo, se encolan.
                        foreach (var hijo in aux.getHijos())
                        {
                            cola.encolar(hijo);
                        }
                    }
                }
                //Y pasamos al siguiente nivel
                nivelActual++;
            }
            return result;
        }
        public void AgregarDato(ArbolGeneral<DatoDistancia> arbol, DatoDistancia dato)
        {
            //Primero se agrega el dato si el arbol esta vacio
            if (arbol.getDatoRaiz() == null)
            {
                arbol = new ArbolGeneral<DatoDistancia>(dato);
            }
            else
            {
                //sino recorro los hijos para ver si hay algun dato igual al ingresado
                foreach (ArbolGeneral<DatoDistancia> hijo in arbol.getHijos())
                {
                    //se calcula la distancia, si es igual a 0
                    //quiere decir que ya hay un dato igual al infresado enotnces no agrega nada

                    int distancia = CalcularDistancia(hijo.getDatoRaiz().texto, dato.texto);
                    if (distancia == 0)
                    {
                        return;
                    }
                }
                //Y si hay distancia, se agrega el dato
                int distanciaRaiz = CalcularDistancia(arbol.getDatoRaiz().texto, dato.texto);
                if (distanciaRaiz > 0)
                {
                    // Crear un nuevo nodo para el dato y agregarlo como hijo del nodo actual
                    arbol.getHijos().Add(new ArbolGeneral<DatoDistancia>(dato));
                }
            }
        }       
        public List<DatoDistancia> Buscar(ArbolGeneral<DatoDistancia> arbol, string elementoABuscar, int umbral, List<DatoDistancia> collected)
            {
            
            if (arbol.getDatoRaiz() == null)
            {
                return collected;
            }
            else
            {
                //calculamos la distancia, si el dato esta entre el umbral se agrega el dato a la coleccion
                int distancia = CalcularDistancia(arbol.getDatoRaiz().texto, elementoABuscar);
                if (distancia <= umbral)
                {
                    collected.Add(arbol.getDatoRaiz());
                }
                //y por medio de recusion hacemos lo mismo con los hijos
                foreach (ArbolGeneral<DatoDistancia> hijo in arbol.getHijos())
                {
                    Buscar(hijo, elementoABuscar, umbral, collected);
                }
            }
            return collected;
        }

    }
}