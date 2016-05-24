using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Model
{
	public class Plasma
	{
		public Texture2D Texture;
		public int Damage;
		public bool Active;
		public Vector2 Position;
		Viewport viewport;
		float projectileMoveSpeed;

		// Get the width of the projectile ship
		public int Width 
		{
			get { return Texture.Width; }
		}

		// Get the height of the projectile ship
		public int Height 
		{
			get { return Texture.Height; }
		}
			
		public void Initialize (Viewport viewport, Texture2D texture, Vector2 position)
		{
			Texture = texture;
			Position = position;
			this.viewport = viewport;

			Active = true;

			Damage = 50;

			projectileMoveSpeed = 5;
		}

		public void Update ()
		{
			// Projectiles always move to the right
			Position.X += projectileMoveSpeed;

			// Deactivate the bullet if it goes out of screen
			if (Position.X + Texture.Width / 2 > viewport.Width)
				Active = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
				new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
		}
	}
}

