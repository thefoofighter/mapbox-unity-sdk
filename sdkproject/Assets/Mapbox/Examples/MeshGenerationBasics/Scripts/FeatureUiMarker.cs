﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.MeshGeneration.Components;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class FeatureUiMarker : MonoBehaviour
{
    [SerializeField]
    private Transform _wrapperMarker;
    [SerializeField]
    private Transform _infoPanel;
    [SerializeField]
    private Text _info;

    private Vector3[] _targetVerts;
    private FeatureBehaviour _selectedFeature;

    void Update()
    {
        Snap();
    }

    internal void Clear()
    {
        gameObject.SetActive(false);
    }

    internal void Show(FeatureBehaviour selectedFeature)
    {
        if(selectedFeature == null)
        {
            Clear();
            return;
        }
        _selectedFeature = selectedFeature;
        transform.position = new Vector3(0, 0, 0);
        var mesh = selectedFeature.GetComponent<MeshFilter>();

        if (mesh != null)
        {
            _targetVerts = mesh.mesh.vertices;
            Snap();
        }
        gameObject.SetActive(true);
    }

    private void Snap()
    {
        if (_targetVerts == null)
            return;

        var left = float.MaxValue;
        var right = float.MinValue;
        var top = float.MinValue;
        var bottom = float.MaxValue;
        foreach (var vert in _targetVerts)
        {
            var pos = Camera.main.WorldToScreenPoint(_selectedFeature.transform.position + (_selectedFeature.transform.lossyScale.x * vert));
            if (pos.x < left)
                left = pos.x;
            else if (pos.x > right)
                right = pos.x;
            if (pos.y > top)
                top = pos.y;
            else if (pos.y < bottom)
                bottom = pos.y;
        }

        _wrapperMarker.position = new Vector2(left - 10, top + 10);
        (_wrapperMarker as RectTransform).sizeDelta = new Vector2(right - left + 20, top - bottom + 20);

        _infoPanel.position = new Vector2(right + 10, top + 10);
        _info.text = string.Join(" \r\n ", _selectedFeature.Data.Properties.Select(x => x.Key + " - " + x.Value.ToString()).ToArray());
    }
}
