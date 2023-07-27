using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Matrix<T> : IEnumerable<T>
{
    //IMPLEMENTAR: ESTRUCTURA INTERNA- DONDE GUARDO LOS DATOS?
    T[] _internal;
    public int _width;
    public int _height;
    public int _capacity;

    public Matrix(int width, int height)
    {
        //IMPLEMENTAR: constructor
        Initial(width, height);
    }
    void Initial(int width, int height)
    {
        _width = width;
        _height = height;
        _capacity = width * height;
        _internal = new T[_capacity];
    }

    public Matrix(T[,] copyFrom)
    {
        //IMPLEMENTAR: crea una version de Matrix a partir de una matriz básica de C#
        Initial(copyFrom.GetLength(0), copyFrom.GetLength(1));
        int width=0;
        int height=0;

        for (int Y = 0; Y < _height; Y++)
        {
            for (int X = 0; X < _width; X++)
            {
               
               this[X, Y] = copyFrom[X, Y];
              
            }
            //if(i>= copyFrom.GetLength(0))
            //{
            //    height++;
            //    width = 0;
            //}
            //else
            //{
            //    width++;
            //}
           
        }
    }

	public Matrix<T> Clone() {
        Matrix<T> aux = new Matrix<T>(Width, Height);
        for (int Y = 0; Y < _height; Y++)
        {
            for (int X = 0; X < _width; X++)
            {

                aux[X, Y] = this[X, Y];
                
            }
          

        }
        //IMPLEMENTAR
        return aux;
    }

	public void SetRangeTo(int x0, int y0, int x1, int y1, T item) {
        //IMPLEMENTAR: iguala todo el rango pasado por parámetro a item
        var indexInicio = x0 + y0 * _width;
        var indexFinit = x1 + y1 * _width;
        for (int i = indexInicio; i < indexFinit; i++)
        {
            _internal[i] = item;
        }

    }

    //Todos los parametros son INCLUYENTES
    public List<T> GetRange(int x0, int y0, int x1, int y1) {
        List<T> l = new List<T>();
       
        var indexInicio = x0 + y0 * _width;
        var indexFinit = x1 + y1 * _width;
        for (int i = y0; i < y1; i++)
        {
            for (int x = x0; x < x1; x++)
            {
                l.Add(this[x,i]);
            }
        }
       
      
        return l;
	}

    //Para poder igualar valores en la matrix a algo
    public T this[int x, int y] {
		get
        {
            //IMPLEMENTAR
            var index = x + y * _width;
            return _internal[index];
           //return default(T);
		}
		set {
            //IMPLEMENTAR
            var index = x + y * _width;
            _internal[index] = value;
        }
	}

    public int Width { get { return _width; } private set { _width = value; } }

    public int Height { get { return _height; } private set { _height = value; } }

    public int Capacity { get { return _capacity; } private set { _capacity = value; } }

    public IEnumerator<T> GetEnumerator()
    {
        //IMPLEMENTAR
        for (int i = 0; i < _capacity; i++)
        {
            yield return _internal[i];
        }
      
       // yield return default(T);
    }

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
