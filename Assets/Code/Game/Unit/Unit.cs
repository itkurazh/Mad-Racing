using System;
using UnityEngine;

public class Unit : Entity
{
    public UnitData Data => _data;
    public UnitView View => _view;
    
    [SerializeField] private UnitView _view;
    [SerializeField] private UnitController _controller;
    private UnitData _data;

    protected override void Awake()
    {
        _data = new UnitData();
        _data.Controller = _controller;
        
        _view.SetData(_data);
    }

    private void Update()
    {
        Data.Position = Data.Controller.Position;
    }

    protected override void Subscribe()
    {
        
    }

    protected override void Unsubscribe()
    {
       
    }
}