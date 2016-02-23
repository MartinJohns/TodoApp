/// <reference path="../../typings/tsd.d.ts" />

import * as React from 'react';
import * as ReactDOM from 'react-dom';


import TodoItem from './todoItem';
import TodoFooter from './todoFooter';
import { ITodo } from '../interfaces.g';
import { ITodoModel } from '../models/todoModel';
import { ALL_TODOS, ACTIVE_TODOS, COMPLETED_TODOS, ENTER_KEY } from "../constants";

interface ITodoAppProps {
    model: ITodoModel;
}

interface ITodoAppState {
    editing?: string;
    nowShowing?: string;
}


export default class TodoApp extends React.Component<ITodoAppProps, ITodoAppState> {
    public state: ITodoAppState;

    constructor(props: ITodoAppProps) {
        super(props);

        this.state = {
            nowShowing: ALL_TODOS,
            editing: null
        };
    }

    private handleNewTodoKeyDown(event: __React.KeyboardEvent): void {
        if (event.keyCode !== ENTER_KEY) {
            return;
        }

        const val: string = ReactDOM.findDOMNode<HTMLInputElement>(this.refs["newField"]).value.trim();
        if (val) {
            this.props.model.addTodo(val);
            ReactDOM.findDOMNode<HTMLInputElement>(this.refs["newField"]).value = '';
        }
    }

    public toggleAll(event: __React.FormEvent) {
        var target: any = event.target;
        var checked = target.checked;
        this.props.model.toggleAll(checked);
    }

    public toggle(todoToToggle: ITodo) {
        this.props.model.toggle(todoToToggle);
    }

    public destroy(todo: ITodo) {
        this.props.model.destroy(todo);
    }

    public edit(todo: ITodo) {
        this.setState({ editing: todo.id });
    }

    public save(todoToSave: ITodo, text: String) {
        this.props.model.save(todoToSave, text);
        this.setState({ editing: null });
    }

    public cancel() {
        this.setState({ editing: null });
    }

    public clearCompleted() {
        this.props.model.clearCompleted();
    }

    public render() {
        let main;
        let footer;

        const todos: ITodo[] = this.props.model.todos;

        const shownTodos = todos.filter(todo => {
            switch (this.state.nowShowing) {
                case ACTIVE_TODOS: return !todo.completed;
                case COMPLETED_TODOS: return todo.completed;
                default: return true;
            }
        });

        const todoItems = shownTodos.map(todo => (
            <TodoItem
                key={todo.id}
                todo={todo}
                onToggle={ this.toggle.bind(this, todo) }
                onDestroy={ this.destroy.bind(this, todo) }
                onEdit={ this.edit.bind(this, todo) }
                onSave={ this.save.bind(this, todo) }
                onCancel={ e => this.cancel() }
            />
        ));

        const activeTodoCount: number = todos.reduce((acc, todo) => todo.completed ? acc : acc + 1, 0);
        const completedCount: number = todos.length - activeTodoCount;

        if (activeTodoCount || completedCount) {
            footer =
                <TodoFooter
                    count={activeTodoCount}
                    completedCount={completedCount}
                    nowShowing={this.state.nowShowing}
                    onClearCompleted={e=> this.clearCompleted() }
                    />;
        }

        if (todos.length) {
            main = (
                <section className="main">
                    <input
                        className="toggle-all"
                        type="checkbox"
                        onChange={ e => this.toggleAll(e) }
                        checked={ activeTodoCount === 0 }
                        />
                    <ul className="todo-list">
                        {todoItems}
                    </ul>
                </section>
            );
        }

        return (
            <div>
                <header className="header">
                    <h1>todos</h1>
                    <input
                        ref="newField"
                        className="new-todo"
                        placeholder="What needs to be done?"
                        onKeyDown={ e => this.handleNewTodoKeyDown(e) }
                        autoFocus={true} />
                </header>
                {main}
                {footer}
            </div>
        );
    }

    private renderTodos() {
        const todoItems = this.props.model.todos.map(todo => (
            <TodoItem
                key={todo.id}
                todo={todo}
                onToggle={ this.toggle.bind(this, todo) }
                onDestroy={ this.destroy.bind(this, todo) }
                onEdit={ this.edit.bind(this, todo) }
                onSave={ this.save.bind(this, todo) }
                onCancel={ e => this.cancel() }
            />
        ));
        return todoItems;
    }
}
