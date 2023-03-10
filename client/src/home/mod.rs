use yew::prelude::*;
#[function_component(Home)]
pub fn home() -> Html {
    html! {
        <div>
        <h1>{"HOME"}</h1>
        <p>{"this is the first post"}</p>
        <img src="https://place-hold.it/900x600"/>
        </div>
    }
}
