
import { ITodo } from '../../interfaces.g';

interface ITodoWithData {
    title?: string;
    completed?: boolean;
}

export function todoWith(todo: ITodo, data: ITodoWithData): ITodo {
    const newTodo: ITodo = {
        id: todo.id,
        title: todo.title,
        completed: todo.completed
    };

    if (data.title !== undefined) {
        newTodo.title = data.title;
    }

    if (data.completed !== undefined) {
        newTodo.completed = data.completed;
    }

    return newTodo;
}
