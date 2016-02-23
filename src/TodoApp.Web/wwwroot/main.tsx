/// <reference path="../typings/tsd.d.ts" />

import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Router, Route, Link, browserHistory } from 'react-router'

import TodoApp from './components/todoApp';
import LocalStorageTodoModel from './models/localStorageTodoModel';
import RemoteTodoModel from './models/remoteTodoModel';

//const todoModel = new TodoModel('react-todos');
const todoModel = new RemoteTodoModel('react-todos', '/api/');

function render() {
    ReactDOM.render(
        <TodoApp model={todoModel} />,
        document.getElementsByClassName('todoapp')[0]
    );
}

todoModel.subscribe(render);
render();

console.log('Todo app started.');
