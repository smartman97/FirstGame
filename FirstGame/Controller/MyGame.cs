using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using FirstGame.Model;
using FirstGame.View;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace FirstGame.controller
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MyGame : Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private Player player;

		// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState; 

		// Image used to display the static background
		private Texture2D mainBackground;

		// Parallaxing Layers
		private ParallaxingBackground bgLayer1;
		private ParallaxingBackground bgLayer2;

		// Enemies
		Texture2D enemyTexture;
		List<Enemy> enemies;

		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// A random number generator
		Random random;

		// Hearts
		Texture2D heartTexture;
		List<Heart> hearts;

		// The rate at which the enemies appear
		TimeSpan heartSpawnTime;
		TimeSpan previousHeartSpawnTime;

		// A random number generator
		Random randomHeart;

		Texture2D projectileTexture;
		List<Projectile> projectiles;

		// The rate of fire of the player laser
		TimeSpan fireTime;
		TimeSpan previousFireTime;

		Texture2D plasmaTexture;
		List<Plasma> plasmas;

		TimeSpan plasmaFireTime;
		TimeSpan previousPlasmaFireTime;

		Texture2D explosionTexture;
		List<Animation> explosions;

		// The sound that is played when a laser is fired
		SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		SoundEffect explosionSound;

		// The music played during gameplay
		Song gameplayMusic;

		//Number that holds the player score
		int score;
		// The font used to display UI elements
		SpriteFont font;

		// A movement speed for the player
		private float playerMoveSpeed;

		public MyGame ()
		{
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			player = new Player ();

			// Set a constant player move speed
			playerMoveSpeed = 8.0f;

			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			// Initialize the enemies list
			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator
			random = new Random();

			// Initialize the hearts list
			hearts = new List<Heart> ();

			// Set the time keepers to zero
			previousHeartSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			heartSpawnTime = TimeSpan.FromSeconds(30f);

			// Initialize our random number generator
			randomHeart = new Random();

			projectiles = new List<Projectile>();
			plasmas = new List<Plasma> ();

			explosions = new List<Animation>();

			//Set player's score to zero
			score = 0;

			// Set the laser to fire every quarter second
			fireTime = TimeSpan.FromSeconds(.15f);
			plasmaFireTime = TimeSpan.FromSeconds(5);
            
			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
				+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);

			// Load the parallaxing background
			bgLayer1.Initialize(Content, "Texture/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Texture/bgLayer2", GraphicsDevice.Viewport.Width, -2);

			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");
			heartTexture = Content.Load<Texture2D>("Texture/heart");

			projectileTexture = Content.Load<Texture2D>("Texture/laser");
			plasmaTexture = Content.Load<Texture2D> ("Texture/plasmaSmall");

			explosionTexture = Content.Load<Texture2D>("Animation/explosion");

			// Load the music
			gameplayMusic = Content.Load<Song>("Sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");

			// Load the score font
			font = Content.Load<SpriteFont>("Font/gameFont");

			// Start the music right away
			PlayMusic(gameplayMusic);

			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif
            
			/// Save the previous state of the keyboard and game pad so we can determinesingle key/button presses
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);

			//Update the player
			UpdatePlayer(gameTime);

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();

			// Update the enemies
			UpdateEnemies(gameTime);

			UpdateHearts (gameTime);

			// Update the collision
			UpdateCollision();

			// Update the projectiles
			UpdateProjectiles();

			UpdatePlasma ();

			// Update the explosions
			UpdateExplosions(gameTime);
            
			base.Update (gameTime);
		}

		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update(gameTime);

			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) ||
				currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) ||
				currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}

			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, (player.Width / 2),GraphicsDevice.Viewport.Width - (player.Width / 2));
			player.Position.Y = MathHelper.Clamp(player.Position.Y, (player.Height / 2),GraphicsDevice.Viewport.Height - (player.Height / 2));

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

				// Play the laser sound
				laserSound.Play();
			}

			if (gameTime.TotalGameTime - previousPlasmaFireTime > plasmaFireTime)
			{
				// Reset our current time
				previousPlasmaFireTime = gameTime.TotalGameTime;

				// Add the plasma, but add it to the front and center of the player
				AddPlasma(player.Position + new Vector2(player.Width / 2, 0));

				// Play the laser sound
				laserSound.Play();
			}

			// reset score if player health goes to zero
			if (player.Health <= 0)
			{
				player.Health = 100;
				score = 0;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.MediumSeaGreen);
            
			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}

			for (int i = 0; i < hearts.Count; i++)
			{
				hearts[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for (int i = 0; i < projectiles.Count; i++)
			{
				projectiles[i].Draw(spriteBatch);
			}

			for (int i = 0; i < plasmas.Count; i++)
			{
				plasmas[i].Draw(spriteBatch);
			}

			// Draw the explosions
			for (int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}

			// Draw the score
			spriteBatch.DrawString(font, "Score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
			// Draw the player health
			spriteBatch.DrawString(font, "Health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

			// Draw the Player
			player.Draw(spriteBatch);

			// Stop drawing
			spriteBatch.End();
            
			base.Draw (gameTime);
		}

		private void AddEnemy()
		{ 
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position); 

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);

						// Play the explosion sound
						explosionSound.Play();

						//Add to the player's score
						score += enemies[i].Value;
					}

					enemies.RemoveAt(i);
				} 
			}
		}

		private void AddHeart()
		{ 
			Heart heart = new Heart (); 

			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + heartTexture.Width / 2, randomHeart.Next(0, GraphicsDevice.Viewport.Height));

			heart.Initialize(heartTexture, position); 

			hearts.Add(heart);
		}

		private void UpdateHearts(GameTime gameTime)
		{
			// Spawn a new heart every 12 seconds
			if (gameTime.TotalGameTime - previousHeartSpawnTime > heartSpawnTime) 
			{
				previousHeartSpawnTime = gameTime.TotalGameTime;

				// Add an Heart
				AddHeart();
			}

			// Update the hearts
			for (int i = hearts.Count - 1; i >= 0; i--) 
			{
				hearts[i].Update(gameTime);

				if (hearts[i].Active == false)
				{
					// If not active and health <= 0
					if (hearts[i].Health <= 0)
					{
						//Add to the player's score
						score += hearts[i].Value;
					}

					hearts.RemoveAt(i);
				} 
			}
		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)(player.Position.X - 35),
				(int)(player.Position.Y),
				player.Width,
				player.Height);

			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width, enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}
			}

			for (int i = 0; i < hearts.Count; i++)
			{
				rectangle2 = new Rectangle((int) hearts[i].Position.X, (int)hearts[i].Position.Y, hearts[i].Width, hearts[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= hearts[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					hearts[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}
			}

			// Projectile vs Enemy Collision
			for (int i = 0; i < projectiles.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)projectiles[i].Position.X - 
						projectiles[i].Width / 2,(int)projectiles[i].Position.Y - 
						projectiles[i].Height / 2,projectiles[i].Width, projectiles[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= projectiles[i].Damage;
						projectiles[i].Active = false;
					}
				}
			}

			for (int i = 0; i < plasmas.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)plasmas[i].Position.X - plasmas[i].Width / 2,(int)plasmas[i].Position.Y - plasmas[i].Height / 2,plasmas[i].Width, plasmas[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2, (int)enemies[j].Position.Y - enemies[j].Height / 2, enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= plasmas[i].Damage;
					}
				}
			}
		}

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, projectileTexture,position); 
			projectiles.Add(projectile);
		}

		private void AddPlasma(Vector2 position)
		{
			Plasma plasma = new Plasma (); 
			plasma.Initialize(GraphicsDevice.Viewport, plasmaTexture,position); 
			plasmas.Add(plasma);
		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = projectiles.Count - 1; i >= 0; i--) 
			{
				projectiles[i].Update();

				if (projectiles[i].Active == false)
				{
					projectiles.RemoveAt(i);
				} 
			}
		}

		private void UpdatePlasma()
		{
			// Update the Projectiles
			for (int i = plasmas.Count - 1; i >= 0; i--) 
			{
				plasmas[i].Update();

				if (plasmas[i].Active == false)
				{
					plasmas.RemoveAt(i);
				} 
			}
		}

		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture,position, 134, 134, 12, 45, Color.White, 1f,false);
			explosions.Add(explosion);
		}

		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}


	}
}

