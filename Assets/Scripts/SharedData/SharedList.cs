/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared list of type T between objects
    /// </summary>
    public abstract class SharedList<T> : ScriptableObject, IEnumerable<T>
    {
        protected abstract List<T> Elements { get; set; }

        public T this[int i]
        {
            get => Elements[i];
            set
            {
                Elements[i] = value;
                OnValueChanged?.Invoke();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event Action OnValueChanged;

        public void AddElement(T element)
        {
            if (Elements == null) Debug.Log("no elements");
            if (element == null) Debug.Log("no element");
            Elements.Add(element);
            OnValueChanged?.Invoke();
        }

        public void Insert(int position, T element)
        {
            Elements.Insert(position, element);
        }

        public void RemoveElement(T element)
        {
            Elements.Remove(element);
            OnValueChanged?.Invoke();
        }

        public void RemoveElement(int index)
        {
            Elements.RemoveAt(index);
            OnValueChanged?.Invoke();
        }

        public void SetTo(List<T> elements)
        {
            Elements = elements;
            OnValueChanged?.Invoke();
        }

        public void Clear()
        {
            Elements = new List<T>();
            OnValueChanged?.Invoke();
        }

        public int Count()
        {
            return Elements.Count;
        }

        public List<T> GetCopyOfElements()
        {
            return new List<T>(Elements);
        }

        public T GetElement(int index)
        {
            return Elements[index];
        }
    }
}