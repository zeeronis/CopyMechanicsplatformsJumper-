using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Platform : PoolledObject
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var rigibody = collision.gameObject.GetComponent<Rigidbody2D>();
            Debug.Log(rigibody.velocity.y);
            if (rigibody.velocity.y > 0 && rigibody.velocity.y < 3)
            {
                GameManager.Instance.SetPlayerPlatform(transform, true);
            }
        }
    }
}
