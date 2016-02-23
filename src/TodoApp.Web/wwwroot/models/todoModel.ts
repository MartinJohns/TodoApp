import { ITodo } from '../interfaces.g';

export interface ITodoModel {
    key: string;
    todos: Array<ITodo>;

    onChanges: Array<() => void>;
    subscribe(onChange: () => void);
    inform();
    addTodo(title: string);
    toggleAll(checked: boolean);
    toggle(todoToToggle);
    destroy(todo);
    save(todoToSave, text);
    clearCompleted();
}
