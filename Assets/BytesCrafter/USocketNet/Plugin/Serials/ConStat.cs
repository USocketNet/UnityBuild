﻿#region License
/*
 * ConStat.cs
 *
 * Copyright (c) 2020 Bytes Crafter
 *
 * Permission is hereby granted to any person obtaining a copy from our store
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software with restriction to the rights to modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to permit 
 * persons to whom the Software is furnished to do so, subject to the following 
 * conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

namespace BytesCrafter.USocketNet
{
	/// <summary>
	/// Conn auth is an enum handler for the current client authentication.
	/// </summary>
	public enum ConStat
	{ 
		/// <summary>
		/// Successfully enter the server. Code: 0
		/// </summary>
		Success, 
		/// <summary>
		/// Application id dont exist on the database. Code: 1
		/// </summary>
		Invalid, 
		/// <summary>
		/// Server found invalid arguments thus rejecting conn. Code: 2
		/// </summary>
		Rejected,

		/// <summary>
		/// Please check your socket host and port. Code: 3
		/// </summary>
		Error,


		
		/// <summary>
		/// Game is currenly inactive and block want new connection. Code: 4
		/// </summary>
		Inactive,
		/// <summary>
		/// Game is currenly at its limit and cant accept new connection Code: 5
		/// </summary>
		Overload,
		/// <summary>
		/// Users account is currently online. Code: 6
		/// </summary>
		Online,

		/// <summary>
		/// USocketClient is currently performing task. Code: 7
		/// </summary>
		Busy,
	}
}