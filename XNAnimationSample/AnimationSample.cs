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
using System.Security.Claims;
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

        private AnimationClip activeAnimationClip;
        private int activeAnimationClipIndex = 0;
        private string interpolationMode = "Linear";

        private SkinnedModel skinnedModel;
        private AnimationController animationController;

        private Camera camera;

        private Ground ground;
        private CharacterController controller;

        public AnimationSample()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,

                PreferMultiSampling = true,
            };

            Content.RootDirectory = "Content";

            //Components.Add (ground);

            controller = new CharacterController(Vector3.Zero);
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
            ground = new Ground(this);

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

                    // effect.AmbientLightColor = new Vector3 (.9f);
                    // effect.DiffuseColor = new Vector3 (.9f);
                    // effect.DirectionalLight0.Direction = Vector3.Left;

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }
            }


            // Create an animation controller and start a clip
            animationController = new AnimationController(skinnedModel.SkeletonBones);
            animationController.Speed = 1f;

            animationController.TranslationInterpolation = InterpolationMode.Linear;
            animationController.OrientationInterpolation = InterpolationMode.Linear;
            animationController.ScaleInterpolation = InterpolationMode.Linear;

            activeAnimationClip = skinnedModel.AnimationClips["idle"];
            animationController.StartClip(activeAnimationClip);

            // Set up the camera.
            var viewport = GraphicsDevice.Viewport;
            camera = new ChaseCamera(new Vector3(0, 0, 0), new Vector3(0, 500, 1000), Vector3.Up, viewport.Width / viewport.Height);
            ground.LoadContent();
            ground.UpdateViewAndProjection(camera.ViewMatrix, camera.ProjectionMatrix);
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

            int animationCount = skinnedModel.AnimationClips.Count;
            // Change the animation clip smoothly by using CrossFade.
            if (keyboardState.IsKeyDown(Keys.Left) &&
                lastKeyboardState.IsKeyUp(Keys.Left))
            {
                activeAnimationClipIndex = (activeAnimationClipIndex - 1);

                if (activeAnimationClipIndex < 0)
                {
                    activeAnimationClipIndex = skinnedModel.AnimationClips.Count - 1;
                }

                animationController.CrossFade(skinnedModel.AnimationClips.Values[activeAnimationClipIndex], TimeSpan.FromSeconds(0.05f));
            }
            else if (keyboardState.IsKeyDown(Keys.Right) &&
                     lastKeyboardState.IsKeyUp(Keys.Right))
            {
                activeAnimationClipIndex = (activeAnimationClipIndex + 1);

                if (activeAnimationClipIndex >= skinnedModel.AnimationClips.Count)
                {
                    activeAnimationClipIndex = 0;
                }

                animationController.CrossFade(skinnedModel.AnimationClips.Values[activeAnimationClipIndex], TimeSpan.FromSeconds(0.05f));
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
            // if (keyboradState.IsKeyDown(Keys.Space) && lastKeyboradState.IsKeyUp(Keys.Space))
            // {
            //     animationController.LoopEnabled = !animationController.LoopEnabled;
            // }

            controller.Update(gameTime);

            var v = controller.Velocity;
            v.Y = 0f; // zero out the vertical component
            var l = v.Length();
            AnimationClip clip = null;
            if (l > 0.1f)
                clip = skinnedModel.AnimationClips["sprint"];
            else if (l > 0f)
                clip = skinnedModel.AnimationClips["walk"];
            else
                clip = skinnedModel.AnimationClips["idle"];

            if (controller.Velocity.Y > 0f)
            {
                clip = skinnedModel.AnimationClips["jump"];
            }

            if (keyboardState.IsKeyDown(Keys.LeftControl))
            {
                clip = skinnedModel.AnimationClips["attack-melee-right"];
            }
            if (lastKeyboardState.IsKeyUp(Keys.LeftControl) && activeAnimationClip == skinnedModel.AnimationClips["attack-melee-right"])
            {
                clip = skinnedModel.AnimationClips["idle"];
            }

            if (activeAnimationClip != clip)
            {
                activeAnimationClip = clip;
                animationController.CrossFade(clip, TimeSpan.FromSeconds(0.05f));
            }
            v = Vector3.Normalize(controller.Velocity);
            if (l > 0f)
                animationController.Speed = (float)v.Length();
            else animationController.Speed = 1f;
            // Update the models animation.
            animationController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            camera.Position = controller.Position;
            camera.Update(gameTime);
            ground.UpdateViewAndProjection(camera.ViewMatrix, camera.ProjectionMatrix);
            base.Update(gameTime);
        }

        public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMax;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            ground.Draw(gameTime);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (ModelMesh mesh in skinnedModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(animationController.SkinnedBoneTransforms);
                    effect.World = Matrix.CreateFromQuaternion(controller.Rotation) * Matrix.CreateTranslation(controller.Position);

                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
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

            string animationName = activeAnimationClip.Name;

            stringBuilder.AppendLine("Press Left/Right to change the current animation");
            stringBuilder.AppendLine(string.Format("    Current Animation : {0}", animationName));

            // stringBuilder.AppendLine();

            // stringBuilder.AppendLine("Press Up/Down to change the animation speed");
            // stringBuilder.Append("    Animation Speed : ");
            // stringBuilder.Append(animationController.Speed);

            // stringBuilder.AppendLine();

            // stringBuilder.AppendLine("\nPress Space to toggle looping");
            // stringBuilder.Append("    Looping : ");
            // stringBuilder.Append(animationController.LoopEnabled);

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("\nPress 1, 2, 3 or 4 to change the type of interpolation");
            stringBuilder.Append("    Interpolation Mode : ");
            stringBuilder.Append(interpolationMode);

            stringBuilder.AppendLine();

            stringBuilder.AppendLine($"Velocity : {controller.Velocity}");
            stringBuilder.AppendLine($"Position : {controller.Position}");

            // stringBuilder.AppendLine("\nThe type of interpolation controls how the animation");
            // stringBuilder.AppendLine("controller blends between two keyframes and also how");
            // stringBuilder.AppendLine("the controller fades between two animations. The effects");
            // stringBuilder.AppendLine("of the different interpolator types can be seen at slow speeds");

            spriteBatch.DrawString(spriteFont, stringBuilder.ToString(), new Vector2(30, 30), Color.White);

            spriteBatch.End();
        }
    }
}
