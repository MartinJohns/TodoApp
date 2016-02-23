
import { ITodo } from '../interfaces.g';
import { ITodoModel } from './todoModel';
import { todoWith } from './lib/todoWith';
import Utils from '../utils';


export default class LocalStorageTodoModel implements ITodoModel {
    public key: string;
    public todos: Array<ITodo>;
    public onChanges: Array<() => void>;

    constructor(key) {
        this.key = key;
        this.todos = Utils.store(key);
        this.onChanges = [];
    }

    public subscribe(onChange: () => void): void {
        this.onChanges.push(onChange);
    }

    public inform(): void {
        Utils.store(this.key, this.todos);
        this.onChanges.forEach(cb => cb());
    }

    public addTodo(title: string): void {
        this.todos = this.todos.concat({
            id: Utils.uuid(),
            title: title,
            completed: false
        });

        this.inform();
    }

    public toggleAll(checked: boolean): void {
        this.todos = this.todos.map<ITodo>((todo: ITodo) => {
            return todoWith(todo, { completed: checked });
        });

        this.inform();
    }

    public toggle(todoToToggle: ITodo): void {
        this.todos = this.todos.map<ITodo>((todo: ITodo) => {
            if (todo !== todoToToggle) return todo;

            return todoWith(todo, { completed: !todo.completed });
        });

        this.inform();
    }

    public destroy(todoToDestroy: ITodo): void {
        this.todos = this.todos.filter((todo) => todo !== todoToDestroy);
        this.inform();
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
