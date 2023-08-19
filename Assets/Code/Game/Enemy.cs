using System;
using Tools;
using TSG.Model;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public event Action<Enemy, GameObject> onImpact = delegate { };
    public event Action<Enemy> onDie = delegate { };
    public float endPoint;
    
    private EnemyModel model;
    public EnemyModel Model => model;
    
    public void Setup(EnemyModel model)
    {
        this.model = model;
        model.die += OnModelDie;
    }

    private void OnModelDie(EnemyModel obj)
    {
        Debug.Log("here");
        onDie(this);
    }

    private void Update()
    {
        var pos = transform.position;
        pos.z -= model.Speed * Time.deltaTime;
        transform.position = pos;
        if (transform.position.z < endPoint)
        {
            model.OffRange();
            Pool.Destroy(this);
            //model.die -= OnModelDie;
        }
    }

    public void TakeDamage(float damage)
    {
        model.TakeDamage(damage);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Invoke");
        onImpact.Invoke(this, other.gameObject);
    }
}