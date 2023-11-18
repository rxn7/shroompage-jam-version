using System;

namespace Game;

public interface IHealth {
    float Health { get; set; }
    Action OnDied { get; set; }
    bool IsDead { get; set; }

    void Damage(float damage) {
        if(IsDead) return; // YOUR ASS IS ALREADY DEAD!

        Health -= damage;
        if(Health <= 0.0f) {
            IsDead = true;
            OnDied?.Invoke();
        }
    }
}