﻿using SamLu.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamLu.RegularExpression.StateMachine
{
    /// <summary>
    /// 定义了正则表达式构造的有限状态机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    public interface IRegexFSMState<T> : IState
    {
        /// <summary>
        /// 获取 <see cref="IRegexFSMState{T}"/> 的转换集。
        /// </summary>
        new ICollection<IRegexFSMTransition<T>> Transitions { get; }

        /// <summary>
        /// 添加指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        bool AttachTransition(IRegexFSMTransition<T> transition);

        /// <summary>
        /// 移除指定的转换。
        /// </summary>
        /// <param name="transition">要添加的转换。</param>
        /// <returns>一个值，指示操作是否成功。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transition"/> 的值为 null 。</exception>
        bool RemoveTransition(IRegexFSMTransition<T> transition);
        
        IEnumerable<IRegexFSMTransition<T>> GetOrderedTransitions();
    }

    /// <summary>
    /// 定义了正则表达式构造的有限状态机的状态应遵循的基本约束。
    /// </summary>
    /// <typeparam name="T">正则表达式处理的数据的类型。</typeparam>
    /// <typeparam name="TTransition">有限状态机的转换的类型。</typeparam>
    public interface IRegexFSMState<T, TTransition> : IRegexFSMState<T>, IState<TTransition>
        where TTransition : IRegexFSMTransition<T>
    {
        /// <summary>
        /// 获取 <see cref="IRegexFSMState{T}"/> 的转换集。
        /// </summary>
        new ICollection<TTransition> Transitions { get; }

        new IEnumerable<TTransition> GetOrderedTransitions();
    }
}
