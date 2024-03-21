use crate::store::Store;
use crate::{helpers, Api};
use crate::{router::Route, User, UserField};
use gloo::console::log;
use gloo_net::http::{Headers, Method, Response};
use serde::de::DeserializeOwned;
use serde::Serialize;
use std::ops::Deref;
use std::rc::Rc;
use wasm_bindgen::JsValue;
use web_sys::{Event, HtmlInputElement};
use yew::{Callback, TargetCast, UseStateHandle};
use yew_router::navigator::Navigator;
use yewdux::Dispatch;

pub fn on_change(user: &UseStateHandle<User>, field: UserField) -> Callback<Event> {
    let user = user.clone();
    Callback::from(move |e: Event| {
        let user_clone = user.deref().clone();
        let input = e.target_dyn_into::<HtmlInputElement>();
        if let Some(value) = input {
            let tmp = user_clone.set(value.value(), &field);
            user.set(tmp);
        }
    })
}

pub fn on_click(
    token: String,
    api: Rc<Api>,
    method: Method,
    navigator: Option<Navigator>,
    dispatch: Option<Dispatch<Store>>,
) {
    let method = method.clone();
    let navigator = navigator.clone();
    let hdrs = helpers::create_auth_header(&token);
    let api = Rc::clone(&api);
    let dispatch = dispatch.clone();

    wasm_bindgen_futures::spawn_local(async move {
        if let Some(res) = api.fetch(Some(hdrs), None, method).await {
            if res.status() == 200 {
                dispatch.map(|d| set_user_data(d, Store::default()));
                navigator.map(|nav| nav.push(&Route::Home));
            }
        }
    });
}

pub fn set_user_data(dispatch: Dispatch<Store>, store: Store) {
    dispatch.reduce_mut(move |s| {
        s.token = store.token;
        s.username = store.username;
        s.authenticated = store.authenticated;
        s.admin = store.admin;
    });
}

pub fn to_jsvalue<T>(body: T) -> JsValue
where
    T: Serialize,
{
    let parsed = serde_json::to_string(&body).unwrap();
    let parsed_body = JsValue::from_str(&parsed);
    parsed_body
}

pub fn create_auth_header(token: &String) -> Headers {
    let auth = format!("Bearer {}", token);
    let hdrs = Headers::new();
    hdrs.append("Authorization", &auth);
    hdrs
}

pub struct CustomCallback;

impl CustomCallback {
    pub fn new<T: 'static>(state: &UseStateHandle<T>) -> Callback<T> {
        let state = state.clone();
        Callback::from(move |req: T| {
            state.set(req);
        })
    }
}

pub async fn emit<T>(callback: &Callback<T>, response: Response)
where
    T: DeserializeOwned,
{
    let txt = response.text().await.unwrap();
    log!("Response: ", &txt);
    callback.emit(serde_json::from_str::<T>(&txt).unwrap());
}
