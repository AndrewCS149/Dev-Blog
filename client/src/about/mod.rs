use yew::prelude::*;
#[function_component(About)]
pub fn about() -> Html {
    html! {
        <div>
            <h1>{"ABOUT"}</h1>
            <p>{"Here is a small description about me"} </p>
        </div>
    }
}
