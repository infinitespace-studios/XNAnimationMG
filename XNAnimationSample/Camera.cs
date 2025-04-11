using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


public abstract class Camera
{
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; protected set; }

    public Matrix ViewMatrix { get; protected set; }
    public Matrix ProjectionMatrix { get; protected set; }

    public abstract void Update(GameTime gameTime);

    protected void Rotate(Vector3 axis, float angle)
    {
        Quaternion rotation = Quaternion.CreateFromAxisAngle(axis, angle);
        Rotation = Quaternion.Normalize(rotation * Rotation);
    }
}

public class ChaseCamera : Camera
{

    Vector3 Offset;

    float chaseSpeed = 10f;
    float rotationSpeed = 5f;
    public ChaseCamera(Vector3 target, Vector3 offset, Vector3 up, float aspectRatio)
    {
        Position = target;
        Offset = offset;
        Position = Position + Offset;
        Rotation = Quaternion.Identity;

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4, aspectRatio, 0.1f, 10000f);
        UpdateViewMatrix();
    }

    private void UpdateViewMatrix()
    {
        Vector3 forward = Vector3.Transform(Vector3.Forward, Rotation);
        ViewMatrix = Matrix.CreateLookAt(Position, Position + forward, Vector3.Up);
    }

    override public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Calculate the desired position
        Vector3 desiredPosition = Position + Offset;

        // Smoothly interpolate the camera's position towards the desired position
        Position = Vector3.Lerp(Position, desiredPosition, chaseSpeed * delta);

        // Calculate the desired direction
        Vector3 desiredDirection = Position - Position;
        desiredDirection.Normalize();

        // Smoothly rotate the camera to face the target
        if (desiredDirection.LengthSquared() > 0)
        {
            Quaternion desiredRotation = Quaternion.CreateFromRotationMatrix(Matrix.CreateLookAt(Position, Position, Vector3.Up));
            Rotation = Quaternion.Slerp(Rotation, desiredRotation, rotationSpeed * delta);
        }

        UpdateViewMatrix();
    }
}

public class DebugCamera : Camera
{
    private float speed = 100f;
    private float rotationSpeed = 1f;

    public DebugCamera(Vector3 position, Vector3 target, Vector3 up, float aspectRatio)
    {
        Position = position;
        Rotation = Quaternion.Identity;

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4, aspectRatio, 0.1f, 5000f);
        UpdateViewMatrix(target, up);
    }

    private void UpdateViewMatrix(Vector3 target, Vector3 up)
    {
        ViewMatrix = Matrix.CreateLookAt(Position, target, up);
    }

    public override void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Keyboard input for movement
        KeyboardState keyboardState = Keyboard.GetState();
        Vector3 movement = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.I))
            movement += Vector3.Forward;
        if (keyboardState.IsKeyDown(Keys.K))
            movement += Vector3.Backward;
        if (keyboardState.IsKeyDown(Keys.J))
            movement += Vector3.Left;
        if (keyboardState.IsKeyDown(Keys.L))
            movement += Vector3.Right;

        // Normalize the movement vector and scale by speed
        if (movement != Vector3.Zero)
        {
            movement.Normalize();
            movement *= speed * delta;

            // Apply rotation to movement vector
            movement = Vector3.Transform(movement, Rotation);
            Position += movement;
        }

        // Keyboard input for rotation
        if (keyboardState.IsKeyDown(Keys.Left))
            Rotate(Vector3.Up, rotationSpeed * delta);
        if (keyboardState.IsKeyDown(Keys.Right))
            Rotate(Vector3.Down, rotationSpeed * delta);
        if (keyboardState.IsKeyDown(Keys.Up))
            Rotate(Vector3.Right, rotationSpeed * delta);
        if (keyboardState.IsKeyDown(Keys.Down))
            Rotate(Vector3.Left, rotationSpeed * delta);

        // Update the view matrix based on the new position and rotation
        Vector3 target = Position + Vector3.Transform(Vector3.Forward, Rotation);
        Vector3 up = Vector3.Transform(Vector3.Up, Rotation);
        UpdateViewMatrix(target, up);
    }
}