using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSize : MonoBehaviour
{
    public delegate void SizeEvent(ESize size);
    public enum ESize {Small, Medium, Big}

    [SerializeField] ESize selectedSize;
    Size currentSize;
    [SerializeField] Size[] sizeVariants = new Size[3];

    Rigidbody2D rb;

    public event SizeEvent OnSizeChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSize = sizeVariants[0];
        if(selectedSize != ESize.Small)
            ChangeSize(selectedSize);
    }

    public void ChangeSize(ESize newSize)
    {
        int index = (int)newSize;
        if(index < sizeVariants.Length && index >= 0)
        {
            if(currentSize != null)
            {
                currentSize.collider.enabled = false;
                currentSize.sprite.SetActive(false);
            }

            currentSize = sizeVariants[index];
            currentSize.collider.enabled = true;
            currentSize.sprite.SetActive(true);
            rb.mass = currentSize.mass;

            selectedSize = newSize;

            if (OnSizeChanged != null)
                OnSizeChanged(selectedSize);
        }
    }

    public void SetNextSize()
    {
        ESize newSize = selectedSize + 1;
        ChangeSize(newSize);
    }

    public void SetPreviousSize()
    {
        ESize newSize = selectedSize - 1;
        ChangeSize(newSize);
    }

    public Collider2D ActivaCollider { get { return currentSize.collider; } }

    public float CurrentMass { get { return currentSize.mass; } }


    [System.Serializable]
    public class Size
    {
        public Collider2D collider;
        public GameObject sprite;
        public float mass;
    }

    public ESize GetSize { get { return selectedSize; } }
}

