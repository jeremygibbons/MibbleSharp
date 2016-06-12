﻿// <copyright file="NFAStateQueue.cs" company="None">
//    <para>
//    This program is free software: you can redistribute it and/or
//    modify it under the terms of the BSD license.</para>
//    <para>
//    This work is distributed in the hope that it will be useful, but
//    WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.</para>
//    <para>
//    See the LICENSE.txt file for more details.</para>
//    Original code as generated by Grammatica 1.6 Copyright (c) 
//    2003-2015 Per Cederberg. All rights reserved.
//    Updates Copyright (c) 2016 Jeremy Gibbons. All rights reserved
// </copyright>

namespace PerCederberg.Grammatica.Runtime.NFA
{
    using System;

    /// <summary><para>
    /// An NFA state queue. This queue is used during processing to
    /// keep track of the current and subsequent NFA states. The
    /// current state is read from the beginning of the queue, and new
    /// states are added at the end. A marker index is used to
    /// separate the current from the subsequent states.
    /// </para><para>
    /// The queue implementation is optimized for quick removal at the
    /// beginning and addition at the end. It will attempt to use a
    /// fixed-size array to store the whole queue, and moves the data
    /// in this array only when absolutely needed. The array is also
    /// enlarged automatically if too many states are being processed
    /// at a single time.
    /// </para></summary>
    internal class NFAStateQueue
    {
        /// <summary>
        /// The state queue array. Will be enlarged as needed.
        /// </summary>
        /// TODO: Replace with list ?
        private NFAState[] queue = new NFAState[2048];

        /// <summary>
        /// The position of the first entry in the queue (inclusive).
        /// </summary>
        private int first = 0;

        /// <summary>
        /// The position just after the last entry in the queue
        /// (exclusive).
        /// </summary>
        private int last = 0;

        /// <summary>
        /// The current queue mark position.
        /// </summary>
        private int mark = 0;

        /// <summary>
        /// Gets a value indicating whether the queue is empty.
        /// </summary>
        public bool Empty
        {
            get
            {
                return this.last <= this.first;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the first entry in the queue has been marked.
        /// </summary>
        public bool Marked
        {
            get
            {
                return this.first == this.mark;
            }
        }

        /// <summary>
        /// Clears this queue. This operation is fast, as it just
        /// resets the queue position indices.
        /// </summary>
        public void Clear()
        {
            this.first = 0;
            this.last = 0;
            this.mark = 0;
        }

        /// <summary>
        /// Marks the end of the queue. This means that the next entry
        /// added to the queue will be marked (when it becomes the
        /// first in the queue). This operation is fast.
        /// </summary>
        public void MarkEnd()
        {
            this.mark = this.last;
        }

        /// <summary>
        /// Removes and returns the first entry in the queue. This
        /// operation is fast, since it will only update the index of
        /// the first entry in the queue.
        /// </summary>
        /// <returns>The previous head of the queue</returns>
        public NFAState RemoveFirst()
        {
            if (this.first < this.last)
            {
                this.first++;
                return this.queue[this.first - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a new entry at the end of the queue. This operation
        /// is mostly fast, unless all the allocated queue space has
        /// already been used.
        /// </summary>
        /// <param name="state">The state to be added</param>
        public void AddLast(NFAState state)
        {
            if (this.last >= this.queue.Length)
            {
                if (this.first <= 0)
                {
                    Array.Resize(ref this.queue, this.queue.Length * 2);
                }
                else
                {
                    Array.Copy(this.queue, this.first, this.queue, 0, this.last - this.first);
                    this.last -= this.first;
                    this.mark -= this.first;
                    this.first = 0;
                }
            }

            this.queue[this.last++] = state;
        }
    }
}