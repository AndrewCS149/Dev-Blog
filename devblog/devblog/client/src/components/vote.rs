use crate::{
    helpers::{self},
    icons::icons::{DownVoteIcon, UpVoteIcon},
    store::Store,
    Api, VoteCount,
};
use gloo_net::http::Method;
use stylist::Style;
use yew::{function_component, html, use_effect_with, use_state, Callback, Html, Properties};
use yewdux::use_store_value;

const STYLE: &str = include_str!("styles/vote.css");

#[derive(Properties, PartialEq)]
pub struct Props {
    pub up_votes: usize,
    pub down_votes: usize,
    pub post_id: u32,
}

#[function_component(Vote)]
pub fn vote(props: &Props) -> Html {
    let style = Style::new(STYLE).unwrap();
    let store = use_store_value::<Store>();
    let down_votes = use_state(|| 0);
    let up_votes = use_state(|| 0);

    let onclick = {
        let id = props.post_id.clone();
        let up_votes = up_votes.clone();
        let down_votes = down_votes.clone();
        Callback::from(move |vote: String| {
            let hdrs = helpers::create_auth_header(&store.token);
            let vote = vote.clone();
            let up_votes = up_votes.clone();
            let down_votes = down_votes.clone();
            wasm_bindgen_futures::spawn_local(async move {
                let response = Api::Vote(id, vote.clone())
                    .fetch(Some(hdrs), None, Method::POST)
                    .await;

                if let Some(res) = response {
                    if res.status() == 200 {
                        let txt = res.text().await.unwrap();
                        let vote_count = serde_json::from_str::<VoteCount>(&txt).unwrap();
                        up_votes.set(vote_count.up);
                        down_votes.set(vote_count.down);
                    }
                }
            });
        })
    };

    let down_votes_clone = down_votes.clone();
    let up_votes_clone = up_votes.clone();
    use_effect_with((props.down_votes, props.up_votes), move |(down, up)| {
        down_votes_clone.set(*down);
        up_votes_clone.set(*up);
    });

    html! {
        <div class={style}>
            <div class="votes">
                <UpVoteIcon vote_onclick={onclick.clone()}/>
                <span>{*up_votes}</span>

                <DownVoteIcon vote_onclick={onclick}/>
                <span>{*down_votes}</span>
            </div>
        </div>
    }
}
