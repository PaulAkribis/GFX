﻿using BulletSharp;
using BulletSharp.Math;
using Genesis.Core;
using OpenObjectLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Physics
{
    /// <summary>
    /// Represents a 3D physics handler responsible for managing physics simulation in a game.
    /// </summary>
    public class PhysicsHandler3D : PhysicHandler
    {
        /// <summary>
        /// Gets or sets the 3D physics world used for simulation.
        /// </summary>
        public DiscreteDynamicsWorld PhysicsWorld { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether physics simulation should be processed.
        /// </summary>
        public bool ProcessPhysics { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the PhysicsHandler3D class with specified PhysicPropeterys.
        /// </summary>
        /// <param name="propeterys">The physics properties containing gravity values.</param>
        public PhysicsHandler3D(PhysicPropeterys propeterys)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = new BulletSharp.Math.Vector3(propeterys.gravityX, propeterys.gravityY, propeterys.gravityZ);
        }

        /// <summary>
        /// Initializes a new instance of the PhysicsHandler3D class with specified gravity values.
        /// </summary>
        /// <param name="gravityX">The X component of gravity.</param>
        /// <param name="gravityY">The Y component of gravity.</param>
        /// <param name="gravityZ">The Z component of gravity.</param>
        public PhysicsHandler3D(float gravityX, float gravityY, float gravityZ)
        {
            var CollisionConfiguration = new DefaultCollisionConfiguration();
            var Dispatcher = new CollisionDispatcher(CollisionConfiguration);
            var Broadphase = new DbvtBroadphase();
            this.PhysicsWorld = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfiguration);
            this.PhysicsWorld.Gravity = new BulletSharp.Math.Vector3(gravityX, gravityY, gravityZ);
        }

        /// <summary>
        /// Processes the physics simulation for the given scene and game.
        /// </summary>
        /// <param name="scene">The current game scene.</param>
        /// <param name="game">The current game instance.</param>
        public override void Process(Scene scene, Game game)
        {
            if (this.ProcessPhysics && this.PhysicsWorld != null)
            {
                this.PhysicsWorld.StepSimulation((float)(game.DeltaTime / 1000));

                int numManifolds = PhysicsWorld.Dispatcher.NumManifolds;
                for (int i = 0; i < numManifolds; i++)
                {
                    PersistentManifold contactManifold = PhysicsWorld.Dispatcher.GetManifoldByIndexInternal(i);
                    CollisionObject obA = contactManifold.Body0 as CollisionObject;
                    CollisionObject obB = contactManifold.Body1 as CollisionObject;

                    if(Callbacks.ContainsKey(obA))
                    {
                        Callbacks[obA](scene, game, obB);
                    }
                }
            }
        }

        /// <summary>
        /// Manages a physics behavior element by adding its RigidBody to the physics world.
        /// </summary>
        /// <param name="rigidBody">The PhysicsBehavior representing the rigid body element.</param>
        public override void ManageElement(PhysicsBehavior rigidBody)
        {
            base.ManageElement(rigidBody);
            PhysicsWorld.AddRigidBody((RigidBody) rigidBody.GetPhysicsObject());
        }
    }
}
