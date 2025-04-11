using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XNAnimationSample;

// public class World {
//     private readonly Random random = new Random();
//     public Vector3[] Positions { get; set; } = new Vector3[10000];
//     public Vector3[] Velocities { get; set; } = new Vector3[10000];

//     public World() {
//         for (int i = 0; i < Positions.Length; i++) {
//             Positions[i] = new Vector3(i, random.NextSingle (), random.NextSingle ());
//             Velocities[i] = new Vector3(1, random.NextSingle (), random.NextSingle ());
//         }
//     }

//     public void Update(GameTime gameTime) {
//         PhysicsSystem.Update(this, gameTime);
//     }
// }

// public class PhysicsSystem {
//     public static void Update(World world, GameTime gameTime) {
//         for (int i = 0; i < world.Positions.Length; i++) {
//             world.Positions[i] += world.Velocities[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;
//         }
//     }
// }
public struct Input {
    public Vector3 MoveDirection;
}

public class NpcInputSystem {
    public static void Update(ref Input input, GameTime gameTime) {
    }
}
public class PlayerInputSystem {
    static KeyboardState lastKeyboardState;
    public static void Update(ref Input input, float walkSpeed, float runSpeed, GameTime gameTime) {
        KeyboardState keyboardState = Keyboard.GetState();
        var run = keyboardState.IsKeyDown(Keys.LeftShift);
        input.MoveDirection = Vector3.Zero;
        if (keyboardState.IsKeyDown(Keys.W))
            input.MoveDirection += Vector3.Forward * (run ? runSpeed : walkSpeed);
        if (keyboardState.IsKeyDown(Keys.S))
            input.MoveDirection += Vector3.Backward * (run ? runSpeed : walkSpeed);
        if (keyboardState.IsKeyDown(Keys.A))
            input.MoveDirection += Vector3.Right * (run ? runSpeed : walkSpeed);
        if (keyboardState.IsKeyDown(Keys.D))
            input.MoveDirection += Vector3.Left * (run ? runSpeed : walkSpeed);
        if (keyboardState.IsKeyDown(Keys.Space))
            input.MoveDirection += Vector3.Up;
        lastKeyboardState = keyboardState;
    }
}

public class CharacterController
{
    private Vector3 position;
    private Vector3 velocity;
    private bool isGrounded;
    private float jumpForce = 2f;
    private float jump = 0f;
    private float walkSpeed = .09f;
    private float sprintSpeed = .15f;
    private float rotationSpeed = MathHelper.Pi * 2f; // Adjust as needed

    public Vector3 Position => position;
    public Quaternion Rotation { get; private set; }

    public Vector3 Velocity => velocity;

    public CharacterController(Vector3 startPosition)
    {
        position = startPosition;
        velocity = Vector3.Zero;
        isGrounded = false;
        Rotation = Quaternion.Identity; // Start with no rotation
    }

    KeyboardState lastKeyboardState;
    //Input input;

    public void Update(GameTime gameTime)
    {
        // Apply movement
        //PlayerInputSystem.Update(ref input, gameTime);

        KeyboardState keyboardState = Keyboard.GetState();
        Vector3 moveDirection = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
            moveDirection += Vector3.Forward;
        if (keyboardState.IsKeyDown(Keys.S))
            moveDirection += Vector3.Backward;
        if (keyboardState.IsKeyDown(Keys.A))
            moveDirection += Vector3.Right;
        if (keyboardState.IsKeyDown(Keys.D))
            moveDirection += Vector3.Left;

        var speed = walkSpeed;
        if (keyboardState.IsKeyDown(Keys.LeftShift))
            speed = sprintSpeed;

        if (moveDirection != Vector3.Zero)
            moveDirection = Vector3.Normalize(moveDirection);
        velocity = moveDirection * speed;

        // Apply gravity
        if (!isGrounded)
        {
            velocity.Y -= 0.98f;// Gravity
        }

        // Rotate the character towards the direction of travel using Lerp
        if (moveDirection != Vector3.Zero)
        {
            var v = velocity;
            // ignore the y component
            v.Y = 0;
            var rotationMatrix = Matrix.CreateLookAt(position, position + (v * 10f), Vector3.Up);
            var targetRotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
            Rotation = Quaternion.Lerp(Rotation, targetRotation, rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        // Check for jumping
        if (isGrounded && keyboardState.IsKeyDown(Keys.Space) && !lastKeyboardState.IsKeyDown(Keys.Space))
        {
            jump = jumpForce;
        }

        if (jump > 0f)
        {
            velocity.Y += jump;
            isGrounded = false;
            jump -= 0.009f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        // hack to make the movement go in the correct X direction.
        velocity.X = -velocity.X;
        // Update position
        position += velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        // Check for ground collision (you'll need to implement this)
        if (position.Y <= 0f && !isGrounded)
        {
            position.Y = 0f;
            isGrounded = true;
            velocity.Y = 0f;
            jump = 0f;
        }
        lastKeyboardState = keyboardState;
    }
}