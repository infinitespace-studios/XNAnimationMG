/*
 * AnimationSample.cs
 * Author: Bruno Evangelista
 * Copyright (c) 2008 Bruno Evangelista. All rights reserved.
 *
 * THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
 * CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XNAnimation;
using XNAnimation.Controllers;

namespace XNAnimationSample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class AnimationSample : Game
    {
        private GraphicsDeviceManager graphics;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private StringBuilder stringBuilder;

        private KeyboardState keyboardState;
        private KeyboardState lastKeyboardState;

        private int activeAnimationClip;
        private string interpolationMode = "Linear";

        private SkinnedModel skinnedModel;
        private AnimationController animationController;

        private float rotation;

        private Matrix view;
        private Matrix projection;

        public AnimationSample()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,

                PreferMultiSampling = true,
            };

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            stringBuilder = new StringBuilder();

            base.Initialize();

            Window.Title = "Kenney.nl";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("Fonts\\Arial");

            // Load the skinned model
            skinnedModel = Content.Load<SkinnedModel>("Models\\character-soldier");


            foreach (ModelMesh mesh in skinnedModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {

                    /*
                    //  texture can be changed:
                    Texture2D axe = Content.Load<Texture2D>("Textures\\axe");
                    Texture2D dwarf = Content.Load<Texture2D>("Textures\\dwarf");
                    effect.Texture = dwarf;
                    if (mesh.Name == "axe")
                    {
                        effect.Texture = axe;
                    }
                    */

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }
            }


            // Create an animation controller and start a clip
            animationController = new AnimationController(skinnedModel.SkeletonBones);
            animationController.Speed = 0.5f;

            animationController.TranslationInterpolation = InterpolationMode.Linear;
            animationController.OrientationInterpolation = InterpolationMode.Linear;
            animationController.ScaleInterpolation = InterpolationMode.Linear;

            //animationController.StartClip(skinnedModel.AnimationClips["Take 001"]);

            activeAnimationClip = 1; // "UkkoArmature|Idle"
            animationController.StartClip(skinnedModel.AnimationClips.Values[activeAnimationClip]);

            // Set up the camera.
            view = Matrix.CreateLookAt(new Vector3(100, 100, 500), new Vector3(0, 30, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1280 / 720, 1, 1000);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            // Exit the sample.
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Change the animation clip smoothly by using CrossFade.
            if (keyboardState.IsKeyDown(Keys.Left) &&
                lastKeyboardState.IsKeyUp(Keys.Left))
            {
                activeAnimationClip = (activeAnimationClip - 1);

                if (activeAnimationClip < 0)
                {
                    activeAnimationClip = skinnedModel.AnimationClips.Count - 1;
                }

                animationController.CrossFade(skinnedModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
            }
            else if (keyboardState.IsKeyDown(Keys.Right) &&
                     lastKeyboardState.IsKeyUp(Keys.Right))
            {
                activeAnimationClip = (activeAnimationClip + 1);

                if (activeAnimationClip >= skinnedModel.AnimationClips.Count)
                {
                    activeAnimationClip = 0;
                }

                animationController.CrossFade(skinnedModel.AnimationClips.Values[activeAnimationClip], TimeSpan.FromSeconds(0.05f));
            }

            // Change the type of interpolation to use between keyframes.
            // The differences are mainly noticeible at an animation speed of 0.1.
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                interpolationMode = "None";

                animationController.TranslationInterpolation = InterpolationMode.None;
                animationController.OrientationInterpolation = InterpolationMode.None;
                animationController.ScaleInterpolation = InterpolationMode.None;
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                interpolationMode = "Linear";

                animationController.TranslationInterpolation = InterpolationMode.Linear;
                animationController.OrientationInterpolation = InterpolationMode.Linear;
                animationController.ScaleInterpolation = InterpolationMode.Linear;
            }
            else if (keyboardState.IsKeyDown(Keys.D3))
            {
                interpolationMode = "Cubic";

                animationController.TranslationInterpolation = InterpolationMode.Cubic;
                animationController.OrientationInterpolation = InterpolationMode.Linear;
                animationController.ScaleInterpolation = InterpolationMode.Cubic;
            }
            else if (keyboardState.IsKeyDown(Keys.D4))
            {
                interpolationMode = "Spherical";

                animationController.TranslationInterpolation = InterpolationMode.Linear;
                animationController.OrientationInterpolation = InterpolationMode.Spherical;
                animationController.ScaleInterpolation = InterpolationMode.Linear;
            }

            // Toggle if the animation will loop or not.
            if (keyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
            {
                animationController.LoopEnabled = !animationController.LoopEnabled;
            }

            // Change the speed of the animation.
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                animationController.Speed += 0.005f;

                animationController.Speed = MathHelper.Clamp(
                    animationController.Speed, 0.1f, 5.0f);
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                animationController.Speed -= 0.005f;

                animationController.Speed = MathHelper.Clamp(
                    animationController.Speed, 0.1f, 5.0f);
            }

            // Update the models rotation.
            rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * 0.4f;
            // Update the models animation.
            animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in skinnedModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(animationController.SkinnedBoneTransforms);
                    effect.World = Matrix.CreateRotationY(rotation);

                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }

            DrawHUD();

            base.Draw(gameTime);
        }

        private void DrawHUD()
        {
            spriteBatch.Begin();

            stringBuilder.Clear();

            string animationName = skinnedModel.AnimationClips.Keys[activeAnimationClip];

            stringBuilder.AppendLine("Press Left/Right to change the current animation");
            stringBuilder.AppendLine(string.Format("    Current Animation : {0}", animationName));

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Press Up/Down to change the animation speed");
            stringBuilder.Append("    Animation Speed : ");
            stringBuilder.Append(animationController.Speed);

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("\nPress Space to toggle looping");
            stringBuilder.Append("    Looping : ");
            stringBuilder.Append(animationController.LoopEnabled);

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("\nPress 1, 2, 3 or 4 to change the type of interpolation");
            stringBuilder.Append("    Interpolation Mode : ");
            stringBuilder.Append(interpolationMode);

            stringBuilder.AppendLine();

            // stringBuilder.AppendLine("\nThe type of interpolation controls how the animation");
            // stringBuilder.AppendLine("controller blends between two keyframes and also how");
            // stringBuilder.AppendLine("the controller fades between two animations. The effects");
            // stringBuilder.AppendLine("of the different interpolator types can be seen at slow speeds");

            spriteBatch.DrawString(spriteFont, stringBuilder.ToString(), new Vector2(30, 30), Color.White);

            spriteBatch.End();
        }
    }
}
