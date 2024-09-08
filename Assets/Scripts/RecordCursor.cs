using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordCursor : MonoBehaviour
{
    [SerializeField] private GameObject _clickEffectPrefab;
    [SerializeField] private float _destroyTime = 1f;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 spawnPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            GameObject clickEffect = Instantiate(_clickEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(clickEffect, _destroyTime);
        }
    }
}