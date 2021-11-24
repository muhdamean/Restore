import { LoadingButton } from "@mui/lab";
import { Avatar, Button, Card, CardActions, CardContent, CardHeader, CardMedia, Typography } from "@mui/material";
import { Link } from "react-router-dom";
import { Product } from "../../app/models/product";
import { useAppDispatch, useAppSelector } from "../../app/store/configureStore";
import { currencyFormat } from "../../app/util/util";
import { addBasketItemAsync } from "../basket/basketSlice";

interface Props{
    product: Product;
}

export default function ProductCard({product}:Props){
  const {status}=useAppSelector(state=>state.basket);
  //const [loading, setLoading]=useState(false);
  //const {setBasket}=useStoreContext();
  const dispatch=useAppDispatch();

  // function handleAddItem(productId: number){
  //     setLoading(true);
  //     agent.Basket.addItem(productId)
  //        .then(basket=> dispatch(setBasket(basket)))
  //       .catch(error=>console.log(error))
  //       .finally(()=>setLoading(false));
  // }
    return  (
        <Card sx={{ maxWidth: 345 }}>
            <CardHeader 
                avatar={
                    <Avatar sx={{bgcolor: 'secondary.main'}}>
                        {product.name.charAt(0).toUpperCase()}
                    </Avatar>
                }
                title={product.name}
                titleTypographyProps={{
                    sx: {fontWeight: 'bold',color:'primary.main'}
                }}
            />
        <CardMedia
          sx={{height:140, backgroundSize:'contain', bgcolor:'primary.light'}}
          image={product.pictureUrl}
          title={product.name}
        />
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {currencyFormat(product.price)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {product.brand} / {product.price}
          </Typography>
        </CardContent>
        <CardActions>
          <LoadingButton loading={status.includes('pendingAddItem' + product.id)} 
          onClick={()=>dispatch(addBasketItemAsync({productId: product.id}))} size="small">Add to Cart</LoadingButton>
          <Button component={Link} to={`/catalog/${product.id}`} size="small">View</Button>
        </CardActions>
      </Card>
        // <ListItem key={product.id}>
        //     <ListItemAvatar>
        //         <Avatar src={product.pictureUrl}></Avatar>
        //     </ListItemAvatar>
        //     <ListItemText>
        //         {product.name} = {product.price}
        //     </ListItemText>
        // </ListItem>
    )
}