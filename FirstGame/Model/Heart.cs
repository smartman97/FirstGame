using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Model
{
	public class Heart
	{
		public Texture2D HeartTexture;

		public Vector2 Position;

		public bool Active;

		public int Health;

		Viewport viewport;

		public int Damage;

		public int Value;

		public int Width
		{
			get { return HeartTexture.Width; } 
		}
			
		public int Height
		{
			get { return HeartTexture.Height; } 
		}
			
		float heartMoveSpeed;

		public void Initialize(Texture2D texture, Vector2 position)
		{
			HeartTexture = texture;

			Position = position;

			Active = true;

			Health = 2;

			Damage = -30;

			heartMoveSpeed = 12f;

			Value = 500;
		}

		public void Update(GameTime gameTime)
		{ 

			Position.X -= heartMoveSpeed;

			if (Position.X < -Width || Health <= 0)
			{
				Active = false;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(HeartTexture, Position, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
		}

	}
}

