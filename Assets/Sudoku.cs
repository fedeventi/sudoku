﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class Sudoku : MonoBehaviour
{
    public Cell prefabCell;
    public Canvas canvas;
    public Text feedback;
    public float stepDuration = 0.05f;
    [Range(5, 70)] public int difficulty = 40;//de 0 a el total de celdas (ancho*largo)

    Matrix<Cell> _board;
    Matrix<int> _createdMatrix;
    List<int> posibles = new List<int>();
    int _smallSide;
    int _bigSide;
    string memory = "";
    string canSolve = "";
    bool canPlayMusic = false;
    List<int> nums = new List<int>();
    int x;
    int y;
    List<Matrix<int>> solution = new List<Matrix<int>>();

    float r = 1.0594f;
    float frequency = 440;
    float gain = 0.5f;
    float increment;
    float phase;
    float samplingF = 48000;
    public int size;
    Matrix<int> validMatrix;
    void Start()
    {
        long mem = System.GC.GetTotalMemory(true);
        feedback.text = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        memory = feedback.text;
        _smallSide = 3;
        _bigSide = _smallSide * 3;
        frequency = frequency * Mathf.Pow(r, 2);
        CreateEmptyBoard();
        ClearBoard();
    }

    void ClearBoard()
    {

        _createdMatrix = new Matrix<int>(_bigSide, _bigSide);

        foreach (var cell in _board)
        {
            cell.number = 0;
            cell.locked = cell.invalid = false;
        }
    }

    void CreateEmptyBoard()
    {
        float spacing = 68f;
        float startX = -spacing * 4f;
        float startY = spacing * 4f;

        _board = new Matrix<Cell>(_bigSide, _bigSide);
        for (int x = 0; x < _board.Width; x++)
        {
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board[x, y] = Instantiate(prefabCell);
                cell.transform.SetParent(canvas.transform, false);
                cell.transform.localPosition = new Vector3(startX + x * spacing, startY - y * spacing, 0);
            }
        }
    }



    //IMPLEMENTAR
    int watchdog = 0;
    bool RecuSolve(Matrix<int> matrixParent, int x, int y, int protectMaxDepth, List<Matrix<int>> solution)
    {
        watchdog--;
        if (watchdog <= 0)
        {
            return false;
        }
        if (x >= matrixParent.Width)
        {
            y++;
            x = 0;
            if (y >= matrixParent.Height)
            {
                return true;
            }
            
        }
        if (_board[x, y].locked)
        {
            return RecuSolve(matrixParent, x+1, y, 1, solution);
        }
        else
        {  
            for (int item = 1; item <= 9; item++)
            {
                if (CanPlaceValue(matrixParent, item, x, y))
                {
                    Matrix<int> result = matrixParent.Clone();
                    result[x, y] = item;
                    solution.Add(result);
                    if (RecuSolve(result, x + 1, y, 1, solution))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

    bool ResolveBrute(Matrix<int> matrixParent,List<Matrix<int>> solution)
    {
        while (matrixParent != validMatrix)
        {
            Debug.Log(matrixParent != validMatrix);
            watchdog--;
            if (watchdog <= 0)
            {
                return false;
            }
            var x = Random.Range(0, matrixParent.Width);
            var y = Random.Range(0, matrixParent.Height);
            if (_board[x, y].locked)
            {
                continue;
            }
            else
            {
                for (int item = 1; item <= 9; item++)
                {
                    if (CanPlaceValue(matrixParent, item, x, y))
                    {
                        Matrix<int> result = matrixParent.Clone();
                        result[x, y] = item;
                        solution.Add(result);
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }
        }
        return true;
    }



    void OnAudioFilterRead(float[] array, int channels)
    {
        if (canPlayMusic)
        {
            increment = frequency * Mathf.PI / samplingF;
            for (int i = 0; i < array.Length; i++)
            {
                phase = phase + increment;
                array[i] = (float)(gain * Mathf.Sin((float)phase));
            }
        }

    }
    void changeFreq(int num)
    {
        frequency = 440 + num * 80;
    }

    //IMPLEMENTAR - punto 3
    IEnumerator ShowSequence(List<Matrix<int>> seq)
    {
        for (int i = 0; i < seq.Count; i++)
            {
                yield return new WaitForSeconds(stepDuration);
                TranslateAllValues(seq[i]);
                feedback.text = "Pasos: " + i + "/" + (seq.Count - 1) + " - " + memory + " - " + canSolve;
            }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            SolvedSudoku();
        else if (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0))
            CreateSudoku();
        /*else if (Input.GetKeyDown(KeyCode.B) || Input.GetMouseButtonDown(2))
            SolvedSudokuBrute();*/
    }

//modificar lo necesario para que funcione.
    void SolvedSudoku()
    {

        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 100000;
        var result = RecuSolve(_createdMatrix, 0, 0, 1, solution);//????
        StartCoroutine(ShowSequence(solution));
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        //???
    
    }
    void SolvedSudokuBrute()
    {

        StopAllCoroutines();
        nums = new List<int>();
        var solution = new List<Matrix<int>>();
        watchdog = 100000;
        var result = ResolveBrute(_createdMatrix, solution);//????
        StartCoroutine(ShowSequence(solution));
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        //???

    }
    void CreateSudoku()
    {
        StopAllCoroutines();
        nums = new List<int>();
        canPlayMusic = false;
        ClearBoard();
        List<Matrix<int>> l = new List<Matrix<int>>();
        GenerateValidLine(_createdMatrix, 0, 0);
        watchdog = 100000;
        var result = RecuSolve(_createdMatrix, 0, 0, 1, l);
        _createdMatrix = l[l.Count - 1].Clone();
        validMatrix = _createdMatrix;
        LockRandomCells();
        ClearUnlocked(_createdMatrix);
        TranslateAllValues(_createdMatrix);
        long mem = System.GC.GetTotalMemory(true);
        memory = string.Format("MEM: {0:f2}MB", mem / (1024f * 1024f));
        canSolve = result ? " VALID" : " INVALID";
        feedback.text = "Pasos: " + l.Count + "/" + l.Count + " - " + memory + " - " + canSolve;

    }
   
    void GenerateValidLine(Matrix<int> mtx, int x, int y)
    {
        int[] aux = new int[9];
        for (int i = 0; i < 9; i++)
        {
            aux[i] = i + 1;
        }
        int numAux = 0;
        for (int j = 0; j < aux.Length; j++)
        {
            int r = 1 + Random.Range(j, aux.Length);
            numAux = aux[r - 1];
            aux[r - 1] = aux[j];
            aux[j] = numAux;
        }
        for (int k = 0; k < aux.Length-1; k++)
        {
            mtx[k, 0] = aux[k];
        }
    }


    void ClearUnlocked(Matrix<int> mtx)
    {
        for (int i = 0; i < _board.Height; i++)
        {
            for (int j = 0; j < _board.Width; j++)
            {
                if (!_board[j, i].locked)
                    mtx[j, i] = Cell.EMPTY;
            }
        }
    }

    void LockRandomCells()
    {
        List<Vector2> posibles = new List<Vector2>();
        for (int i = 0; i < _board.Height; i++)
        {
            for (int j = 0; j < _board.Width; j++)
            {
                if (!_board[j, i].locked)
                    posibles.Add(new Vector2(j, i));
            }
        }
        for (int k = 0; k < 82 - difficulty; k++)
        {
            int r = Random.Range(0, posibles.Count);
            _board[(int)posibles[r].x, (int)posibles[r].y].locked = true;
            posibles.RemoveAt(r);
        }
    }

    void TranslateAllValues(Matrix<int> matrix)
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                _board[x, y].number = matrix[x, y];
            }
        }
    }

    void TranslateSpecific(int value, int x, int y)
    {
        _board[x, y].number = value;
    }

    void TranslateRange(int x0, int y0, int xf, int yf)
    {
        for (int x = x0; x < xf; x++)
        {
            for (int y = y0; y < yf; y++)
            {
                _board[x, y].number = _createdMatrix[x, y];
            }
        }
    }
    void CreateNew()
    {
        _createdMatrix = new Matrix<int>(Tests.validBoards[1]);
        TranslateAllValues(_createdMatrix);
    }

    bool CanPlaceValue(Matrix<int> mtx, int value, int x, int y)
    {
        List<int> fila = new List<int>();
        List<int> columna = new List<int>();
        List<int> area = new List<int>();
        List<int> total = new List<int>();

        Vector2 cuadrante = Vector2.zero;

        for (int i = 0; i < mtx.Height; i++)
        {
            for (int j = 0; j < mtx.Width; j++)
            {
                if (i != y && j == x) columna.Add(mtx[j, i]);
                else if (i == y && j != x) fila.Add(mtx[j, i]);
            }
        }



        cuadrante.x = (int)(x / 3);

        if (x < 3)
            cuadrante.x = 0;
        else if (x < 6)
            cuadrante.x = 3;
        else
            cuadrante.x = 6;

        if (y < 3)
            cuadrante.y = 0;
        else if (y < 6)
            cuadrante.y = 3;
        else
            cuadrante.y = 6;

        area = mtx.GetRange((int)cuadrante.x, (int)cuadrante.y, (int)cuadrante.x + 3, (int)cuadrante.y + 3);
        total.AddRange(fila);
        total.AddRange(columna);
        total.AddRange(area);
        total = FilterZeros(total);

        if (total.Contains(value))
            return false;
        else
            return true;
    }


    List<int> FilterZeros(List<int> list)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != 0) aux.Add(list[i]);
        }
        return aux;
    }
   
  
}
