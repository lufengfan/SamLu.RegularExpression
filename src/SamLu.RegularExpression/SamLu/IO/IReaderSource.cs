﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamLu.IO
{
	/// <summary>
	/// 表示源节点读取器的接口。
	/// </summary>
	/// <typeparam name="TNode"></typeparam>
	public interface IReaderSource<out TNode>
	{
		/// <summary>
		/// 读取下一个节点，并返回。
		/// </summary>
		/// <returns>下一个节点。</returns>
		TNode Read();

        /// <summary>
        /// 获取下一个节点，并返回，但内部指针不前移。
        /// </summary>
        /// <returns>下一个节点。</returns>
        TNode Peek();

		/// <summary>
		/// 获取一个值，指示是否有下一个节点未读。
		/// </summary>
		/// <returns>一个值，指示是否有下一个节点未读。为 false 时指示已经读到结尾。</returns>
		bool HasNext();
	}
}
