using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.View;

namespace FirstGame.Model
{
	public class Player
	{
		private int score;
		private bool active;
		private int health;
		private Animation playerAnimation;

		public Texture2D PlayerTexture;
		public Vector2 Position;

		public Animation PlayerAnimation
		{
			get { return playerAnimation; }
			set { playerAnimation = value; }
		}

		public int Health
		{
			get { return health; }
			set { health = value; }
		}

		// Get the width of the player ship
		public int Width
		{
			get { return PlayerAnimation.FrameWidth; }
		}

		// Get the height of the player ship
		public int Height
		{
			get { return PlayerAnimation.FrameHeight; }
		}
			
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		public int Score
		{
			get { return score; }
			set { score = value; }
		}

		public void Initialize(Texture2D texture, Vector2 position)
		{
			this.active = true;
			this.health = 100;
			this.score = 0;
			this.PlayerTexture = texture;
			this.Position = position;
		}

		// Initialize the player
		public void Initialize(Animation animation, Vector2 position)
		{
			PlayerAnimation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			// Set the player health
			Health = 100;
		}

		// Update the player animation
		public void Update(GameTime gameTime)
		{
			PlayerAnimation.Position = Position;
			PlayerAnimation.Update(gameTime);
		}

		// Draw the player
		public void Draw(SpriteBatch spriteBatch)
		{
			PlayerAnimation.Draw(spriteBatch);
		}
	}
}

