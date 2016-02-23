/// <reference path="../../typings/tsd.d.ts" />

import Utils from '../utils';
import TodoService from '../services/todoService.g';
import { ITodoModel } from './todoModel';
import { ITodo } from '../interfaces.g';
import { todoWith } from './lib/todoWith';

export default class RemoteTodoApp implements ITodoModel {
    public key: string;
    public todos: Array<ITodo>;
    public onChanges: Array<() => void>;
    private todoService: TodoService;

    constructor(key: string, private baseAddress: string) {
        this.key = key;
        this.todos = [];
        this.onChanges = [];
        this.todoService = new TodoService(baseAddress);

        this.todoService.todoGetAll()
            .then(todos => {
                this.todos = todos;
                this.inform();
            });
    }

    public subscribe(onChange: () => void): void {
        this.onChanges.push(onChange);
    }

    public inform(): void {
        this.onChanges.forEach(cb => cb());
    }

    public addTodo(title: string): void {
        const newTodo: ITodo = {
            id: Utils.uuid(),
            title: title,
            completed: false
        };

        this.todoService.todoAdd(newTodo)
            .then(() => {
                this.todos = this.todos.concat(newTodo);
                this.inform();
            });
    }

    public toggleAll(checked: boolean): void {
        this.todos.forEach((todo: ITodo) => {
            this.toggle(todo);
        });
    }

    public toggle(todoToToggle: ITodo): void {
        const updatedTodo: ITodo = todoWith(todoToToggle, { completed: !todoToToggle.completed });
        this.todoService.todoUpdate(todoToToggle.id, updatedTodo)
            .then(() => {
                todoToToggle.completed = !todoToToggle.completed;
                this.inform();
            });
    }

    public destroy(todoToDestroy: ITodo): void {
        this.todoService.todoDelete(todoToDestroy.id)
            .then(() => {
                this.todos = this.todos.filter((todo) => todo !== todoToDestroy);
                this.inform();
            });
    }

    public save(todoToSave: ITodo, text: string): void {
        this.todos = this.todos.map<ITodo>((todo: ITodo) => {
            if (todo !== todoToSave) return todo;

            return todoWith(todo, { title: text });
        });

        this.inform();
    }

    public clearCompleted(): void {
        this.todos = this.todos.filter(todo => !todo.completed);

        this.inform();
    }
}
