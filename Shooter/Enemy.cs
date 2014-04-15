﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using System.Drawing;
using Tao.OpenGl;

namespace Shooter
{
    public class Enemy : Entity
    {
        static readonly double HitFlashTime = 0.25;
        double _scale = 0.3;
        public int Health { get; set; }
        double _hitFlashCountDown = 0;
        BulletManager _bulletManager;
        Texture _bulletTexture;
        EffectsManager _effectsManager;
        public Path Path { get; set; }

        public double MaxTimeToShoot { get; set; }
        public double MinTimeToShoot { get; set; }
        Random _random = new Random();
        double _shootCountDown;

        PlayerCharacter _playerCharacter;

        public void RestartShootCountDown()
        {
            _shootCountDown = MinTimeToShoot + (_random.NextDouble() * MaxTimeToShoot);
        }

        public bool IsDead
        {
            get { return Health == 0; }
        }

        public Enemy(TextureManager textureManager, EffectsManager effectsManager, BulletManager bulletManager, PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
            _bulletManager = bulletManager;
            _bulletTexture = textureManager.Get("bullet");
            MaxTimeToShoot = 12;
            MinTimeToShoot = 1;
            RestartShootCountDown();

            _effectsManager = effectsManager;
            Health = 50; // default health value.
            _sprite.Texture = textureManager.Get("enemy_ship");
            _sprite.SetScale(_scale, _scale);
            _sprite.SetRotation(Math.PI); // make it face the player
            _sprite.SetPosition(200, 0); // put it somewhere easy to see
        }

       
        public void Update(double elapsedTime)
        {
            _shootCountDown = _shootCountDown - elapsedTime;
            if (_shootCountDown <= 0)
            {
                Bullet bullet = new Bullet(_bulletTexture);
                Vector currentPosition = _sprite.GetPosition();
                Vector bulletDir = _playerCharacter.GetPosition() - currentPosition;
                bulletDir = Vector.Normalize(bulletDir);
                bullet.Direction = bulletDir;

             
                bullet.Speed = 350;
                //bullet.Direction = new Vector(-1, 0, 0);
                bullet.SetPosition(_sprite.GetPosition());
                bullet.SetColor(new Engine.Color(1, 0, 0, 1));
                _bulletManager.EnemyShoot(bullet);
                RestartShootCountDown();
            }

            if (Path != null)
            {
                Path.UpdatePosition(elapsedTime, this);
            }
            if (_hitFlashCountDown != 0)
            {
                _hitFlashCountDown = Math.Max(0, _hitFlashCountDown - elapsedTime);
                double scaledTime = 1 - (_hitFlashCountDown / HitFlashTime);
                _sprite.SetColor(new Engine.Color(1, 1, (float)scaledTime, 1));
            }

        }

        public void Render(Renderer renderer)
        {
            renderer.DrawSprite(_sprite);
           // Render_Debug();
        }

        internal void OnCollision(PlayerCharacter player)
        {
            // Handle collision with player.
        }


        internal void OnCollision(Bullet bullet)
        {
            // If the ship is already dead then ignore any more bullets.
            if (Health == 0)
            {
                return;
            }

            Health = Math.Max(0, Health - 25);
            _hitFlashCountDown = HitFlashTime; // half
            _sprite.SetColor(new Engine.Color(1, 1, 0, 1));

            if (Health == 0)
            {
                OnDestroyed();
            }

        }

        private void OnDestroyed()
        {
            _effectsManager.AddExplosion(_sprite.GetPosition());
        }

        internal void SetPosition(Vector position)
        {
            _sprite.SetPosition(position);
        }
    }

}
